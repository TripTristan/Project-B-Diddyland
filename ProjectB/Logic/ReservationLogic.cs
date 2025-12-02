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

    public List<Session> GetAvailableSessions()
    {
        var all = _sessionAccess.GetAllSessions();
        return all.Where(s => s.CurrentBookings < _sessionAccess.GetCapacityBySession(s)).ToList();
    }

    public bool CanBookSession(int sessionId, int quantity)
    {
        var session = _sessionAccess.GetSessionById(sessionId);
        if (session == null) return false;

        return session.CurrentBookings + quantity <= _sessionAccess.GetCapacityBySession(session);
    }

    public decimal CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber, int qty)
    {
        var session = _sessionAccess.GetSessionById(sessionId)
                     ?? throw new ArgumentException("Invalid session ID.");

        if (session.CurrentBookings + 1 > _sessionAccess.GetCapacityBySession(session))
            throw new InvalidOperationException("Not enough available seats.");

        decimal basePrice = 15;
        var (discount, finalPrice) = CalculateDiscountedPrice(basePrice, age);

        var booking = new ReservationModel(
            orderNumber,
            sessionId,
            qty,
            customer,
            DateTime.Now,
            basePrice,
            discount,
            finalPrice,
            "Reservation");

        _reservationAccess.AddBooking(booking);

        session.CurrentBookings += 1;
        _sessionAccess.UpdateSession(session);

        Console.WriteLine($"Ticket booked for age {age}, price: {finalPrice:C} (discount: {discount * 100}%)");
        return finalPrice;
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

    public (decimal discount, decimal finalPrice) CalculateDiscountedPrice(decimal basePrice, int age)
    {
        decimal discount = 0m;
        if (age <= 12) discount = 0.5m;
        else if (age >= 65) discount = 0.3m;

        decimal finalPrice = basePrice * (1 - discount);
        return (discount, finalPrice);
    }
}
