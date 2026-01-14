using System;
using System.Collections.Generic;
using System.Linq;

public class FastPassLogic
{
    public class Confirmation
    {
        public string OrderNumber { get; set; } = "";
        public string Attraction { get; set; } = "";
        public string Type { get; set; } = "FastPass";
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public int Quantity { get; set; }
        public double PricePerPerson { get; set; }
        public double TotalPrice { get; set; }
    }

    private readonly SessionAccess _sessionAccess;
    private readonly ReservationLogic _reservationLogic;
    private readonly ReservationAccess _reservationAccess;
    private readonly AttractiesAccess _attractiesAccess;

    public FastPassLogic(
        SessionAccess sessionAccess,
        ReservationLogic reservationLogic,
        ReservationAccess reservationAccess,
        AttractiesAccess attractiesAccess)
    {
        _sessionAccess = sessionAccess;
        _reservationLogic = reservationLogic;
        _reservationAccess = reservationAccess;
        _attractiesAccess = attractiesAccess;
    }

    public List<SessionModel> GetAvailableFastPassSessions(int attractionId, DateTime day, string location)
    {
        var sessionsForDay = _sessionAccess.EnsureFastPassSessionsForAttractionAndDate(attractionId, day, location);

        return sessionsForDay
            .Where(s => s.Capacity > 0)
            .OrderBy(s => s.Time)
            .ToList();
    }

    public Confirmation BookFastPass(long sessionId, int quantity, UserModel? user, string location)
    {
        var session = _sessionAccess.GetSessionById(sessionId)
                      ?? throw new ArgumentException("Session not found.");

        if (session.SessionType != 1)
            throw new InvalidOperationException("This session is not a FastPass session.");

        if (!_reservationLogic.CanBookSession(sessionId, quantity))
            throw new InvalidOperationException("Not enough capacity for this timeslot.");

        const double basePrice = 10.0;
        double original = basePrice * quantity;

        var orderNo = _reservationLogic.GenerateOrderNumber(user);

        var reservation = new ReservationModel(
            orderNo,
            sessionId,
            quantity,
            user ?? new UserModel { Id = 0, Name = "Guest" },
            DateTime.Now.Ticks,
            original,
            1);

        _reservationAccess.AddBooking(reservation);

        session.Capacity -= quantity;
        _sessionAccess.UpdateSession(session);

        string attractionName = "";
        try
        {
            var attr = _attractiesAccess.GetAll(location)
                        .FirstOrDefault(a => a.ID == session.AttractionId);
            if (attr != null) attractionName = attr.Name;
        }
        catch
        {
            attractionName = "";
        }

        return new Confirmation
        {
            OrderNumber = orderNo,
            Type = "FastPass",
            Attraction = attractionName,
            Date = new DateTime(session.Date).ToString("yyyy-MM-dd"),
            Time = new TimeSpan(session.Time).ToString(@"hh\:mm"),
            Quantity = quantity,
            PricePerPerson = basePrice,
            TotalPrice = original
        };
    }
}
