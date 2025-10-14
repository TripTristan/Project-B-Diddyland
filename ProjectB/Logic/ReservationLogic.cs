using System;
using System.Collections.Generic;

public class ReservationLogic
{
    private readonly SessionAccess _sessionRepo;
    private readonly ReservationsAccess _bookingRepo;

    public ReservationLogic(SessionAccess sessionRepo, ReservationsAccess bookingRepo)
    {
        _sessionRepo = sessionRepo;
        _bookingRepo = bookingRepo;
    }


    public List<Session> GetAvailableSessions()
    {
        List<Session> allSessions = _sessionRepo.GetAllSessions();
        List<Session> available = new List<Session>();

        foreach (var session in allSessions)
        {
            if (session.CurrentBookings < session.MaxCapacity)
            {
                available.Add(session);
            }
        }

        return available;
    }


    public bool CanBookSession(int sessionId, int quantity) 
    {
        Session? session = _sessionRepo.GetSessionById(sessionId);
        if (session == null) return false;
        return session.CurrentBookings + quantity <= session.MaxCapacity;
    }


    // Shopping-cart // create booking
    public string CreateBooking(int sessionId, int quantity, UserModel? customer, string OrderNumber)
    {
        Session? session = _sessionRepo.GetSessionById(sessionId);
        if (session == null)
        {
            throw new ArgumentException("Invalid session ID.");
        }

        if (session.CurrentBookings + quantity > session.MaxCapacity)
        {
            throw new InvalidOperationException("Not enough available seats.");
        }

        session.CurrentBookings += quantity;
        _sessionRepo.UpdateSession(session);

        Booking booking = new Booking
        {
            OrderNumber = orderNumber,
            SessionId = sessionId,
            Quantity = quantity,
            Customer = customer,
            BookingDate = DateTime.Now
        };

        _bookingRepo.AddBooking(booking);
        return orderNumber;
    }


    private string GenerateOrderNumber(UserModel? customerInfo)
    {
        UserModel? _customerInfo = LoginStatus.CurrentUserInfo;
        Random random = new Random();
        int randomNumber = random.Next(1000, 9999);

        if (customerInfo != null)
        {
            return $"ORD-{customerInfo.Id}-{customerInfo.Username}-{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";
        }
        return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-GUEST-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";
    }
}