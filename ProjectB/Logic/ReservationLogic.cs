using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationLogic
{
    private readonly SessionAccess _sessionAccess;
    private readonly ReservationAccess _reservationAccess;

    public ReservationLogic(SessionAccess sessionAccess, ReservationAccess reservationAccess)
    {
        _sessionAccess = sessionAccess;
        _reservationAccess = reservationAccess;
    }


    public void CreateSingleTicketBooking(long sessionId, int qty, UserModel? customer, double price)
    {
        SessionModel session = _sessionAccess.GetSessionById(sessionId);
        if (session.Capacity - qty < 5)
            throw new InvalidOperationException("Not enough available seats.");


        ReservationModel booking = new ReservationModel(GenerateOrderNumber(customer), sessionId, qty, customer, DateTime.Now.Ticks, price, 0);

        _reservationAccess.AddBooking(booking);

        session.Capacity -= qty;
        _sessionAccess.UpdateSession(session);
          Console.WriteLine($"Ticket booked for {customer.Name}, price: {price:C}");
    }

    public bool CanBookSession(long sessionId, int qty)
    {
        SessionModel session = _sessionAccess.GetSessionById(sessionId);
        return session.Capacity - qty < 5;
    }

    public string GenerateOrderNumber(UserModel? customerInfo)
    {
        var random = new Random();
        int randomNumber = random.Next(1000, 9999);
        string suffix = $"{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";

        if (customerInfo != null)
            return $"ORD-{customerInfo.Id}-{customerInfo.Username}-{suffix}";

        return $"ORD-GUEST-{suffix}";
    }


    public bool CheckForTimeslotsOnDate(DateTime date)
    {
        if (_sessionAccess.GetAllSessionsForDate(date.Ticks).Count != 3)
        {
            return false;
        }
        return true;
    }

    public void PopulateTimeslots(DateTime date)
    {
        for (int i = 0; i<3; i++)
        {
            SessionModel session = new(_sessionAccess.NextId(), date.Ticks, i+1, 35);
            _sessionAccess.Insert(session);
        }
    }

    public string AvailabilityFormatter(SessionModel Session)
    {
        string prefix;
        string suffix;
        List<string> timeslots = new() {"", "09:00-13:00", "13:00-17:00", "17:00-21:00"};
        switch (Session.Capacity)
        {
            case > 10:
                prefix = "✅ ";
                suffix = $" | {Session.Capacity-5} | slots are still available";
                break;
            case 0:
                prefix = " | ⭕ ";
                suffix = $"FULL";
                break;
            case <= 5:
                prefix = "💫 ";
                suffix = $" | {Session.Capacity} | VIP slots left";
                break;
            case > 5:
                prefix = "⚠️ ";
                suffix = $" | {Session.Capacity-5} | LIMITED SLOTS AVAILABLE ";
                break;
            default:
                prefix = "⭕ ";
                suffix = $"ERROR";
                break;
        }

        return $"{prefix} {timeslots[(int)Session.Time]} {suffix}";
    }

    public List<SessionModel> GetSessionsByDate(DateTime date)
    {
        return _sessionAccess.GetAllSessionsForDate(date.Ticks);
    }

    public double CalculatePriceForGuests(List<int> guest)
    {
        return ((guest[0]*5) + (guest[1]*15) + (guest[2]*7.50));
    }

}
