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

    public static List<Session> GetAvailableFastPassSessions(int attractionId, DateTime day, string location)
    {
        var sessionsForDay = SessionAccess.EnsureSessionsForAttractionAndDate(attractionId, day, location);

        return sessionsForDay
            .Where(s => 
                s.Location.Equals(location, StringComparison.OrdinalIgnoreCase) &&
                s.CurrentBookings < SessionAccess.GetCapacityBySession(s)
            )
            .OrderBy(s => s.Time)
            .ToList();
    }

    public static Confirmation BookFastPass(int sessionId, int quantity, UserModel? user, string location)
    {
        var session = SessionAccess.GetSessionById(sessionId)
                    ?? throw new ArgumentException("Session not found.");

        var attraction = AttractiesAccess.GetById(session.AttractionID)
                        ?? throw new Exception("Attraction not found.");

        if (!string.Equals(session.Location, attraction.Location, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("FastPass session does not belong to the same park as the attraction.");

        if (!ReservationLogic.CanBookSession(sessionId, quantity))
            throw new InvalidOperationException("Not enough capacity for this timeslot.");

        const decimal basePrice = 10m;
        decimal original = basePrice * quantity;
        decimal discount = 0m;
        decimal final = original;

        var orderNo = ReservationLogic.GenerateOrderNumber(user);

        var reservation = new ReservationModel(
            orderNo,
            sessionId,
            quantity,
            user ?? new UserModel { Id = 0, Name = "Guest" },
            DateTime.Now,
            original,
            discount,
            final,
            "FastPass"
        );

        ReservationAccess.AddBooking(reservation);

        session.CurrentBookings += quantity;
        SessionAccess.UpdateSession(session);

        return new Confirmation
        {
            OrderNumber = orderNo,
            Attraction = attraction.Name,
            Type = "FastPass",
            Date = session.Date,
            Time = session.Time,
            Quantity = quantity,
            PricePerPerson = basePrice,
            TotalPrice = final
        };
    }
}
