public static class ReservationLogic
{

    public static List<Session> GetAvailableSessions()
    {
        var all = SessionAccess.GetAllSessions();
        return all.Where(s => s.CurrentBookings < SessionAccess.GetCapacityBySession(s)).ToList();
    }

    public static bool CanBookSession(int sessionId, int quantity)
    {
        var session = SessionAccess.GetSessionById(sessionId);
        if (session == null) return false;
        return session.CurrentBookings + quantity <= SessionAccess.GetCapacityBySession(session);
    }

    // Called once per ticket from UI:
    public static decimal CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber, int qty)
    {

        var session = SessionAccess.GetSessionById(sessionId)
                     ?? throw new ArgumentException("Invalid session ID.");
        if (session.CurrentBookings + 1 > SessionAccess.GetCapacityBySession(session))
            throw new InvalidOperationException("Not enough available seats.");

        // price & discount
        // decimal basePrice = session.PricePerPerson;
        decimal basePrice = 15;
        var (discount, finalPrice) = CalculateDiscountedPrice(basePrice, age);

        // persist ticket
        ReservationModel booking = new(orderNumber, sessionId, qty, customer, DateTime.Now,  basePrice, discount, finalPrice);

        ReservationAccess.AddBooking(booking);

        // update capacity
        session.CurrentBookings += 1;
        SessionAccess.UpdateSession(session);

        Console.WriteLine($"Ticket booked for age {age}, price: {finalPrice:C} (discount: {discount * 100}%)");
        return finalPrice;
    }

    public static string GenerateOrderNumber(UserModel? customerInfo)
    {
        var random = new Random();
        int randomNumber = random.Next(1000, 9999);
        string suffix = $"{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";

        if (customerInfo != null)
            return $"ORD-{customerInfo.Id}-{customerInfo.Username}-{suffix}";

        return $"ORD-GUEST-{suffix}";
    }

    public static (decimal discount, decimal finalPrice) CalculateDiscountedPrice(decimal basePrice, int age)
    {
        decimal discount = 0m;
        if (age <= 12) discount = 0.5m;     // children
        else if (age >= 65) discount = 0.3m; // seniors

        decimal finalPrice = basePrice * (1 - discount);
        return (discount, finalPrice);
    }
}
