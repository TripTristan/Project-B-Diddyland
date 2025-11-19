public static class GroupReservationLogic
{
    public const int MIN_GROUP_SIZE = 20;
    private const decimal TICKET_PRICE = 40m;

    public static async Task<(bool isValid, GroupType type)> ValidateGroupTypeAsync(string input)
    {
        if (!int.TryParse(input, out int choice)) return (false, GroupType.None);

        return choice switch
        {
            0 => (true, GroupType.None),
            1 => (true, GroupType.School),
            2 => (true, GroupType.Company),
            _ => (false, GroupType.None)
        };
    }

    public static async Task<ValidationResult> ValidateGroupSizeAsync(int size)
    {
        return size >= MIN_GROUP_SIZE
            ? ValidationResult.Success()
            : ValidationResult.Fail($"Minimum {MIN_GROUP_SIZE} people required");
    }

    public static async Task<List<SessionAvailabilityDto>> GetAvailableSessionsAsync(
        int groupSize, ISessionRepository sessionRepo)
    {
        var sessions = await sessionRepo.GetAllAsync();
        return sessions
            .Where(s => s.CurrentBookings + groupSize <= s.MaxCapacity)
            .Select(s => new SessionAvailabilityDto
            {
                Id = s.Id,
                Date = s.Date,
                Time = s.Time,
                AvailableSeats = s.MaxCapacity - s.CurrentBookings,
                BasisPrice = s.BasisPrice
            })
            .ToList();
    }

    public static async Task<GroupReservationDto> CreateReservationAsync(
        GroupType type, string orgName, string contactPerson, string contactEmail,
        string contactPhone, int groupSize, int sessionId, ISessionRepository sessionRepo, User currentUser)
    {
        var session = await sessionRepo.GetByIdAsync(sessionId);
        if (session == null) throw new ArgumentException("Invalid session");

        decimal discount = CalculateGroupDiscount(groupSize);
        decimal totalBase = groupSize * session.BasisPrice;
        decimal finalPrice = totalBase * (1 - discount);

        return new GroupReservationDto
        {
            OrderNumber = GenerateOrderNumber(currentUser),
            GroupType = type,
            OrganizationName = orgName,
            ContactPerson = contactPerson,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            GroupSize = groupSize,
            SessionId = sessionId,
            Discount = discount,
            FinalPrice = finalPrice
        };
    }

    public static async Task<bool> ProcessPaymentAsync(GroupReservationDto reservation, PaymentMethods method,
        IOrderRepository orderRepo, ISessionRepository sessionRepo)
    {
        bool success = new Random().Next(100) < 95;

        if (success)
        {
            var order = new OrderGroup(reservation.OrderNumber, 0, DateTime.Now, reservation.FinalPrice,
                reservation.GroupType, reservation.GroupSize, reservation.Discount);
            await orderRepo.CreateAsync(order);

            var session = await sessionRepo.GetByIdAsync(reservation.SessionId);
            if (session != null)
            {
                session.CurrentBookings += reservation.GroupSize;
                await sessionRepo.UpdateAsync(session);
            }
        }

        return success;
    }

    private static decimal CalculateGroupDiscount(int groupSize)
    {
        if (groupSize >= 50) return 0.25m;
        if (groupSize >= 30) return 0.2m;
        if (groupSize >= 20) return 0.15m;
        return 0m;
    }

    public static string GenerateOrderNumber(UserModel? customerInfo)
    {
        var random = new Random();
        int randomNumber = random.Next(1000, 9999);
        string suffix = $"{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";

        if (customerInfo != null)
            return $"GRP-ORD-{customerInfo.Id}-{customerInfo.Username}-{suffix}";

        return $"GRP-ORD-GUEST-{suffix}";
    }
}