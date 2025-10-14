public class BookingService
{
    private readonly SessionRepository _sessionRepo = new SessionRepository();

    public List<Session> GetAvailableSessions() => _sessionRepo.GetAvailableSessions();

    public string CreateBooking(int sessionId, string date, string period)
    {
        if (!_sessionRepo.UpdateBookingCount(sessionId))
            return null; 

        var orderNumber = Guid.NewGuid().ToString();
        var booking = new Booking
        {
            OrderNumber = orderNumber,
            SessionId = sessionId,
            BookingDate = date,
            Period = period,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        if (_sessionRepo.InsertBooking(booking))
            return orderNumber;

        return null;
    }
}