using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationLogic
{
    private readonly SessionAccess _sessionAccess;
    private readonly ReservationAccess _reservationAccess;

    public ReservationLogic(SessionAccess sessionAccess, ReservationAccess reservationAccess)
    {
<<<<<<< HEAD
        var all = SessionAccess.GetAllSessions();
        return all.Where(s => s.CurrentBookings < SessionAccess.GetCapacityBySession(s) && s.Date.Ticks >= DateTime.Now.Ticks).ToList();
=======
        _sessionAccess = sessionAccess;
        _reservationAccess = reservationAccess;
>>>>>>> main
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

<<<<<<< HEAD
    // Called once per ticket from UI:
    public static double CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber, int qty)
=======
    public decimal CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber, int qty)
>>>>>>> main
    {
        var session = _sessionAccess.GetSessionById(sessionId)
                     ?? throw new ArgumentException("Invalid session ID.");

        if (session.CurrentBookings + 1 > _sessionAccess.GetCapacityBySession(session))
            throw new InvalidOperationException("Not enough available seats.");

<<<<<<< HEAD
        // price & discount
        // decimal basePrice = session.PricePerPerson;
        double  basePrice = 15.0;
        var (discount, finalPrice) = CalculateDiscountedPrice(basePrice, age);

        ReservationModel booking = new ReservationModel
        {
            OrderNumber   = orderNumber,
            SessionId     = sessionId,
            Quantity      = qty,
            CustomerID    = customer.Id,
            BookingDate   = DateTime.Now.Ticks,
            OriginalPrice = basePrice,
            Discount      = discount,
            FinalPrice    = finalPrice
        };
=======
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
>>>>>>> main

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

<<<<<<< HEAD
    public static (double discount, double finalPrice) CalculateDiscountedPrice(double basePrice, int age)
    {
        double discount = 0.0;
        if (age <= 12) discount = 0.5;     // children
        else if (age >= 65) discount = 0.3; // seniors
=======
    public (decimal discount, decimal finalPrice) CalculateDiscountedPrice(decimal basePrice, int age)
    {
        decimal discount = 0m;
        if (age <= 12) discount = 0.5m;
        else if (age >= 65) discount = 0.3m;
>>>>>>> main

        double finalPrice = basePrice * (1 - discount);
        return (discount, finalPrice);
    }

    public static string GetAttractionNameByAttractionID(int id)
    {
        return AttractionAccess.GetById(id).Name;
    }

    public static int GetAvailableSpotsForSession(Session sesh)
    {
        return SessionAccess.GetCapacityBySession(sesh) - sesh.CurrentBookings;
    }
}
