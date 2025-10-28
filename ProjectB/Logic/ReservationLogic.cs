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
        var all = _sessionRepo.GetAllSessions();
        return all.Where(s => s.CurrentBookings < s.MaxCapacity).ToList();
    }

    public bool CanBookSession(int sessionId, int quantity)
    {
        var session = _sessionRepo.GetSessionById(sessionId);
        if (session == null) return false;
        return session.CurrentBookings + quantity <= session.MaxCapacity;
    }

    // Called once per ticket from UI:
    public decimal CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber)
    {
        var session = _sessionRepo.GetSessionById(sessionId)
                     ?? throw new ArgumentException("Invalid session ID.");

        if (session.CurrentBookings + 1 > session.MaxCapacity)
            throw new InvalidOperationException("Not enough available seats.");

        // price & discount
        decimal basePrice = session.PricePerPerson;
        var (discount, finalPrice) = CalculateDiscountedPrice(basePrice, age);

        // persist ticket
        var booking = new ReservationModel
        {
            OrderNumber = orderNumber,
            SessionId = sessionId,
            Quantity = 1,
            BookingDate = DateTime.Now,
            Customer = customer,
            OriginalPrice = basePrice,
            Discount = discount,
            FinalPrice = finalPrice
        };

        _bookingRepo.AddBooking(booking);

        // update capacity
        session.CurrentBookings += 1;
        _sessionRepo.UpdateSession(session);

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
        if (age <= 12) discount = 0.5m;     // children
        else if (age >= 65) discount = 0.3m; // seniors

        decimal finalPrice = basePrice * (1 - discount);
        return (discount, finalPrice);
    }
}
