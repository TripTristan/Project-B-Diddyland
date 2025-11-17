public class GroupReservationService
{
    private const int MIN_GROUP_SIZE = 20; // 20 minimum group size // For all group types
    private const decimal TICKET_PRICE = 40m;

    private readonly UserModel? _customerInfo = LoginStatus.CurrentUserInfo;
    
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

    public List<SessionModel> GetAvailableShowtimes(int groupSize)
    {
        return _availableShowtimes
            .Where(s => s.AvailableSeats >= groupSize)
            .OrderBy(s => s.Time)
            .ToList();
    }

    public (decimal totalBase, decimal totalFinal) CalculateTotalPrice(int groupSize, decimal discountForGroupSize, decimal basePriceUnit)
    {
        var totalBase = groupSize * basePriceUnit;
        decimal totalFinal = totalBase * (1 - discountForGroupSize);

        return (Math.Round(totalBase, 2), Math.Round(totalFinal, 2));
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
        string contactPerson, 
        string contactEmail,
        string contactPhone,
        int size,
        SessionModel session)
    {
        var sesion = _availableShowtimes.FirstOrDefault(s => s.Id == session.Id);
        if (showtime == null)
            throw new ArgumentException("Invalid showtime selected");


        decimal basePriceUnit = showtime.Price;
        decimal discountForGroupSize = _service.getDiscountForGroupSize(type, size);
        (decimal totalBase, decimal totalFinal) = CalculateTotalPrice(size, discountForGroupSize, basePriceUnit);

       _customerInfo = LoginStatus.CurrentUserInfo;
        var orderNumber = GenerateOrderNumber(_customerInfo);

        return  new GroupReservationDetails
        {
            OrderNumber = orderNumber,
            CustomerId = _customerInfo.Id,
            OrderDate = DateTime.Now,
            OriginalPrice = totalBase,
            Discount = discountForGroupSize,
            FinalPrice = totalFinal,
            Status = false,                     // "Pending Payment"
        };


        // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        bool CreateReservation = ChoiceHelper("Do you want to create the reservation?", "y", "n");

        if (!CreateReservation) return null;

        var orderNumber = GenerateOrderNumber(_customerInfo);

        var reservation = new GroupReservationDetails
        {
            OrderNumber = orderNumber,
            CustomerId = _customerInfo.Id,
            OrderDate = DateTime.Now,
            Subtotal = totalBase,
            Discount = discountForGroupSize,
            Total = totalFinal,
            Status = false,                     // "Pending Payment"
        };
        _repository.Save(reservation);

        bool Payment = ChoiceHelper("Do you want to pay for the reservation?", "y", "n");

        if (!Payment) return null;

        // var paymentResult = ProcessPayment("Credit Card", totalFinal);
        // if (paymentResult.Success)
        // {
        //     reservation.PaymentStatus = true;
        //     _repository.Save(reservation);
        //     return reservation;
        // }
        // else
        // {
        //     return null;
        // }


        



        for (int i = 0; i < size; i++)
        {
            var ticket = new Ticket
            {
                TicketNr = GenerateTicketNr(),
                SesionId = sessionId,
                SeatNumber = i + 1,
                Price = basePriceUnit,
                Discount = discountForGroupSize,
                TotalPrice = totalFinal
            };
            _repository.Save(ticket);
        }



        // Table 
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
            Status = false, // "Pending Payment"
        };
        
        _repository.Save(details);
        return details;
              // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
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

    public static bool ChoiceHelper(string message, string yesOption, string noOption)
    {
        while (true)
        {
            Console.WriteLine($"{message} (y/n):\n y - {yesOption}\n n - {noOption}");
            string choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y") return true;
            if (choice == "n") return false;

            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        }
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
}