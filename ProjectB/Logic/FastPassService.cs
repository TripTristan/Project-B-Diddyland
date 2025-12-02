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
        public decimal PricePerPerson { get; set; }
        public decimal TotalPrice { get; set; }
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

    public List<Session> GetAvailableFastPassSessions(int attractionId, DateTime day)
    {
        var sessionsForDay = _sessionAccess.EnsureSessionsForAttractionAndDate(attractionId, day);
        return sessionsForDay
            .Where(s => s.CurrentBookings < _sessionAccess.GetCapacityBySession(s))
            .OrderBy(s => s.Time)
            .ToList();
    }

    public Confirmation BookFastPass(int sessionId, int quantity, UserModel? user)
    {
        if (!_reservationLogic.CanBookSession(sessionId, quantity))
            throw new InvalidOperationException("Not enough capacity for this timeslot.");

        var session = _sessionAccess.GetSessionById(sessionId)
                      ?? throw new ArgumentException("Session not found.");

        const decimal basePrice = 10m;
        decimal original = basePrice * quantity;
        decimal discount = 0m;
        decimal final = original;

        var orderNo = _reservationLogic.GenerateOrderNumber(user);
        var reservation = new ReservationModel(
            orderNo,
            sessionId,
            quantity,
            user ?? new UserModel { Id = 0, Name = "Guest" },
            DateTime.Now,
            original,
            discount,
            final,
            "FastPass");

        _reservationAccess.AddBooking(reservation);

        session.CurrentBookings += quantity;
        _sessionAccess.UpdateSession(session);

        var attraction = _attractiesAccess.GetById(session.AttractionID);

        return new Confirmation
        {
            OrderNumber = orderNo,
            Attraction = attraction?.Name ?? $"Attraction #{session.AttractionID}",
            Type = "FastPass",
            Date = session.Date,
            Time = session.Time,
            Quantity = quantity,
            PricePerPerson = basePrice,
            TotalPrice = final
        };
    }
}
