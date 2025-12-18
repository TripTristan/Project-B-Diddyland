using System;
using System.Collections.Generic;

public class AdminReservationLogic
{
    private readonly AdminReservationAccess _access;
    private readonly SessionAccess _sessionAccess;
    private readonly ReservationLogic _reservationLogic;

    public AdminReservationLogic(
        AdminReservationAccess access,
        SessionAccess sessionAccess,
        ReservationLogic reservationLogic)
    {
        _access = access;
        _sessionAccess = sessionAccess;
        _reservationLogic = reservationLogic;
    }

    public List<UserModel> GetAllUsers()
    => _access.GetAllUsers();
    public List<BookingModel> GetAll()
        => _access.GetAllBookings();

    public List<BookingModel> GetByUser(int userId)
        => _access.GetBookingsByUser(userId);

    public List<BookingModel> GetBySessionDate(long dateTicks)
        => _access.GetBookingsBySessionDate(dateTicks);

    public BookingModel GetByOrder(string orderNumber)
    {
        return _access.GetByOrder(orderNumber)
            ?? throw new Exception("Booking not found.");
    }

    public void CreateForUser(
        int userId,
        int sessionId,
        int qty,
        decimal price,
        bool isFastPass)
    {
        var session = _sessionAccess.GetSessionById(sessionId)
            ?? throw new Exception("Session not found.");

        if (!_reservationLogic.CanBookSession(sessionId, qty))
            throw new Exception("Not enough capacity.");

        string orderNo =
            $"ORD-ADMIN-{userId}-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4]}";

        var booking = new BookingModel
        {
            OrderNumber = orderNo,
            SessionId = sessionId,
            Quantity = qty,
            CustomerId = userId,
            BookingDate = DateTime.Now.Ticks.ToString(),
            Price = price,
            Type = isFastPass ? "FastPass" : "Reservation"
        };

        _access.Insert(booking);

        session.Capacity -= qty;
        _sessionAccess.UpdateSession(session);
    }

    public void CreateReservationForUser(UserModel user, long sessionId, int qty, double finalPrice)
    {
        _reservationLogic.CreateSingleTicketBooking(
            sessionId,
            qty,
            user,
            finalPrice
        );
    }


    public void DeleteReservation(string orderNumber)
    {
        var booking = _access.GetByOrder(orderNumber)
            ?? throw new Exception("Booking not found.");

        var session = _sessionAccess.GetSessionById(booking.SessionId);
        if (session != null)
        {
            session.Capacity += booking.Quantity;
            _sessionAccess.UpdateSession(session);
        }

        _access.Delete(orderNumber);
    }

    public void EditReservation(
        string orderNumber,
        int newSessionId,
        int newQty,
        decimal newPrice,
        bool isFastPass)
    {
        var booking = _access.GetByOrder(orderNumber)
            ?? throw new Exception("Booking not found.");

        var oldSession = _sessionAccess.GetSessionById(booking.SessionId)
            ?? throw new Exception("Old session missing.");

        var newSession = _sessionAccess.GetSessionById(newSessionId)
            ?? throw new Exception("New session missing.");

        oldSession.Capacity += booking.Quantity;
        _sessionAccess.UpdateSession(oldSession);

        if (!_reservationLogic.CanBookSession(newSessionId, newQty))
            throw new Exception("Not enough capacity.");

        newSession.Capacity -= newQty;
        _sessionAccess.UpdateSession(newSession);

        booking.SessionId = newSessionId;
        booking.Quantity = newQty;
        booking.Price = newPrice;
        booking.Type = isFastPass ? "FastPass" : "Reservation";

        _access.Update(booking);
    }
}
