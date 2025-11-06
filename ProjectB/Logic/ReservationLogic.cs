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

    // hier only age discount Calculate
    public (decimal, decimal, decimal) CreateSingleTicketBooking(int sessionId, int age, UserModel? customer, string orderNumber, int ticketQuantity)
    {
        var session = _sessionRepo.GetSessionById(sessionId)
                    ?? throw new ArgumentException("Invalid session ID.");

        if (session.CurrentBookings + 1 > session.MaxCapacity)
            throw new InvalidOperationException("Not enough available seats.");

        decimal basePrice = session.PricePerPerson;
        string Ticketnr = generateTicketNumber(sessionId, );

        var (discount, baseprice, finalPrice) = CalculateAgePricePerTicket(age); 

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

        session.CurrentBookings += 1;
        _sessionRepo.UpdateSession(session);

        Console.WriteLine($"Ticket booked for age {age}, price: {finalPrice:C} (Age discount: {discount * 100:F0}%)");
        return (discount, basePrice, finalPrice);
    }

    public (decimal discount, decimal basePrice, decimal finalPrice)CalculateAgePricePerTicket(int age)
    {
        decimal basePrice = _bookingRepo.GetBasisTicketPrice();
        OfferManagementLogic offerLogic = new OfferManagementLogic();
        decimal ageDiscount = offerLogic.CalculateRuleTypeAageDiscount(age);
        decimal finalPrice = basePrice * (1 - ageDiscount);
        return (ageDiscount,basePrice, finalPrice);
    }

    public (Dictionary<string, decimal> discount, decimal TicketQuantityDiscount, decimal TotalPricesOrder )CalculateOfferDiscountedPrice(int totalQuantityofTicket, decimal vtotalPrice, UserModel? customerInfo)
    {
        DateTime? bd = customerInfo == null
            ? null
            : DateTime.TryParseExact(customerInfo.DateOfBirth, "dd-MM-yyyy",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out var tmp)
            ? tmp
            : null;

        Dictionary<string, decimal> discount = OfferManagementLogic.ApplyOffers(
                                    new OfferAccess().GetAll(),
                                    "",
                                    totalQuantityofTicket,
                                    customerInfo,
                                    bd);

        decimal offerDiscount = discount.Values.Sum();                   
        
        decimal totalfinalPrice = vtotalPrice * (1 - offerDiscount);


        return (discount, offerDiscount, totalfinalPrice);
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


    //#############################################################################################################
    private (decimal discount, decimal finalPrice) CalculateDiscountedPrice(decimal basePrice, int age)
    {
        decimal ageDiscount = 0m;
        if (age <= 12) ageDiscount = 0.5m;
        else if (age >= 65) ageDiscount = 0.3m;

        decimal final = basePrice * (1 - bestDiscount);
        return (bestDiscount, final);
    }
    //#############################################################################################################
}
