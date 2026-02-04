using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationLogic
{
    private readonly SessionAccess _sessionAccess;
    private readonly ReservationAccess _reservationAccess;
    private readonly AttractiesAccess _attractiesAccess;

    public const int MaxNormalReservation = 10;
    public const int MinGroupReservation = 10;
    public const int MaxGroupReservation = 30;
    public const double GroupDiscountRate = 0.20;

    public ReservationLogic(Dependencies ctx)
    {
        if (ctx == null)
            return;
        _sessionAccess = ctx.sessionAccess;
        _reservationAccess = ctx.reservationAccess;
        _attractiesAccess = ctx.attractiesAccess;
    }

    public void CreateSingleTicketBooking(long sessionId, int qty, UserModel? customer, double price)
    {
        SessionModel session = _sessionAccess.GetSessionById(sessionId);

        if (session.Capacity - qty < 5)
            throw new InvalidOperationException("Not enough available seats.");

        ReservationModel booking = new ReservationModel(
            GenerateOrderNumber(customer),
            sessionId,
            qty,
            customer,
            DateTime.Now.Ticks,
            price,
            0
        );

        _reservationAccess.AddBooking(booking);

        session.Capacity -= qty;
        _sessionAccess.UpdateSession(session);
    }

    public IEnumerable<AttractieModel> GetAttractions(string location)
    {
        return _attractiesAccess.GetAll()
            .Where(a => a.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void ValidateReservationType(int totalGuests, ReservationType type)
    {
        if (totalGuests < 0)
            throw new ArgumentOutOfRangeException(nameof(totalGuests), "Total guests cannot be negative.");

        if (type == ReservationType.Normal && totalGuests > MaxNormalReservation)
            throw new InvalidOperationException("Normal reservations allow up to 10 people.");

        if (type == ReservationType.Group &&
            (totalGuests < MinGroupReservation || totalGuests > MaxGroupReservation))
            throw new InvalidOperationException("Group reservations require 10–30 people.");
    }


    public double ApplyGroupDiscount(double basePrice)
    {
        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative.");

        return basePrice * (1 - GroupDiscountRate);
    }


    public double CalculatePriceForGuests(List<int> guests)
    {
        if (guests == null || guests.Count < 3)
            throw new ArgumentException("Guests must contain at least 3 values.", nameof(guests));

        if (guests[0] < 0 || guests[1] < 0 || guests[2] < 0)
            throw new ArgumentOutOfRangeException(nameof(guests), "Guest counts cannot be negative.");

        return (guests[0] * 5) + (guests[1] * 15) + (guests[2] * 7.50);
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
        return _sessionAccess.GetAllSessionsForDate(date.Ticks).Count == 3;
    }

    public void PopulateTimeslots(DateTime date)
    {
        for (int i = 0; i < 3; i++)
        {
            SessionModel session = new(
                _sessionAccess.NextId(),
                date.Ticks,
                i + 1,
                35
            );
            _sessionAccess.Insert(session);
        }
    }

    public List<SessionModel> GetSessionsByDate(DateTime date)
    {
        return _sessionAccess.GetAllSessionsForDate(date.Ticks);
    }

    public string AvailabilityFormatter(SessionModel session)
    {
        List<string> timeslots = new() { "", "09:00-13:00", "13:00-17:00", "17:00-21:00" };

        int t = (int)session.Time;
        string slot = (t >= 0 && t < timeslots.Count) ? timeslots[t] : "UNKNOWN";

        if (session.Capacity > 10)
            return $"✅ {slot} | {session.Capacity - 5} slots available";

        if (session.Capacity <= 5 && session.Capacity > 0)
            return $"💫 {slot} | {session.Capacity} VIP slots left";

        if (session.Capacity == 0)
            return $"⭕ {slot} | FULL";

        return $"⚠️ {slot} | LIMITED";
    }


    public bool CanBookSession(long sessionId, int qty)
    {
        SessionModel session = _sessionAccess.GetSessionById(sessionId)
            ?? throw new ArgumentException("Session not found.");

        return session.Capacity - qty >= 5;
    }

}
