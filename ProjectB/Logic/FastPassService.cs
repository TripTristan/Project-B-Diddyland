using System;
using System.Collections.Generic;
using System.Linq;

public static class FastPassLogic
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

    public static List<Session> GetAvailableFastPassSessions(int attractionId, DateTime day)
    {
        var sessionsForDay = SessionAccess.EnsureSessionsForAttractionAndDate(attractionId, day);
        return sessionsForDay
            .Where(s => s.CurrentBookings < SessionAccess.GetCapacityBySession(s))
            .OrderBy(s => s.Time)
            .ToList();
    }

    public static Confirmation BookFastPass(int sessionId, int quantity, UserModel? user)
    {
        if (!ReservationLogic.CanBookSession(sessionId, quantity))
            throw new InvalidOperationException("Not enough capacity for this timeslot.");

        var session = SessionAccess.GetSessionById(sessionId)
                      ?? throw new ArgumentException("Session not found.");

        const decimal basePrice = 10m;
        decimal original = basePrice * quantity;
        decimal discount = 0m;
        decimal final = original;

        var orderNo = ReservationLogic.GenerateOrderNumber(user);
        var reservation = new ReservationModel(orderNo, sessionId, quantity,
            user ?? new UserModel { Id = 0, Name = "Guest" },
            DateTime.Now, original, discount, final);

        ReservationAccess.AddBooking(reservation);

        session.CurrentBookings += quantity;
        SessionAccess.UpdateSession(session);

        var attraction = AttractiesAccess.GetById(session.AttractionID);

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
