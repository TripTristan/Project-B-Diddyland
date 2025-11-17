public class GroupReservationService
{
    private const int MIN_GROUP_SIZE = 30;
    private const decimal TICKET_PRICE = 40m;
    
    private readonly IReservationRepository _repository;

    public GroupReservationService(IReservationRepository repository)
    {
        _repository = repository;
    }

    public (bool isValid, GroupType type) ValidateGroupTypeChoice(string input)
    {
        if (!int.TryParse(input, out int choice)) 
            return (false, GroupType.None);
        
        if (choice == 0) return (true, GroupType.None);
        if (choice == 1) return (true, GroupType.School);
        if (choice == 2) return (true, GroupType.Company);
        
        return (false, GroupType.None);
    }

    public ValidationResult ValidateGroupSize(int size)
    {
        return size >= MIN_GROUP_SIZE 
            ? ValidationResult.Success() 
            : ValidationResult.Fail($"The group size must be at least {MIN_GROUP_SIZE} people");
    }

    public List<Showtime> GetAvailableShowtimes(int groupSize)
    {
        return _availableShowtimes
            .Where(s => s.AvailableSeats >= groupSize)
            .OrderBy(s => s.Time)
            .ToList();
    }

    public decimal CalculateTotalPrice(int groupSize, decimal basePrice)
    {
        var total = groupSize * basePrice;
        if (groupSize >= 50)
        {
            total *= (1 - LARGE_GROUP_DISCOUNT);
        }
        return Math.Round(total, 2);
    }

    public PaymentResult ProcessPayment(string paymentMethod, decimal amount)
    {
        return new PaymentResult 
        { 
            Success = true, 
            TransactionId = Guid.NewGuid(),
            PaymentMethod = paymentMethod,
            AmountPaid = amount,
            PaymentDate = DateTime.Now
        };
    }

    public GroupReservationDetails CreateReservation(
        GroupType type, 
        string orgName, 
        string contact, 
        int size,
        int showtimeId,
        string contactEmail,
        string contactPhone)
    {
        var showtime = _availableShowtimes.FirstOrDefault(s => s.Id == showtimeId);
        if (showtime == null)
            throw new ArgumentException("Invalid showtime selected");

        var basePrice = showtime.Price;
        var totalPrice = CalculateTotalPrice(size, basePrice);
        var hasDiscount = size >= 50;

        var details = new GroupReservationDetails
        {
            Id = GenerateReservationId(),
            OrganizationName = orgName,
            ContactPerson = contact,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            GroupType = type,
            GroupSize = size,
            ShowtimeId = showtimeId,
            Showtime = showtime.Time,
            BasePricePerPerson = basePrice,
            TotalPrice = totalPrice,
            Discount = hasDiscount ? LARGE_GROUP_DISCOUNT * 100 : 0,
            ReservationDate = DateTime.Now,
            Status = "Pending Payment"
        };
        
        _repository.Save(details);
        return details;
    }


    private int GenerateReservationId()
    {
        return new Random().Next(1000, 9999);
    }


    public List<string> GetDisclaimerTerms(GroupType type)
    {
        if (type == GroupType.School)
        {
            return new List<string>
            {
                "1. All students must be accompanied by a school teacher or authorized guardian.",
                "2. A complete student list must be submitted 10 days prior to the visit. inclusief (name,age,gender...)",
                "3. Each student must provide a waiver signed by their parent.",
            };
        }
        return new List<string>();
    }
}