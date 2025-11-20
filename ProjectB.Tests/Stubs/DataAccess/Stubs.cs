using MyProject.BLL;

public static class AttractiesAccess
{
    private static readonly List<AttractieModel> Items =
    [
        new AttractieModel(1, "Coaster", "Thrill", 120, 20),
        new AttractieModel(2, "Carousel", "Family", 0, 30)
    ];

    public static IEnumerable<AttractieModel> GetAll() => Items;
    public static AttractieModel? GetById(int id) => Items.FirstOrDefault(a => a.ID == id);
    public static void Insert(AttractieModel model) => Items.Add(model);
    public static void Update(AttractieModel model)
    {
        var existing = GetById(model.ID);
        if (existing == null) return;
        existing.Name = model.Name;
        existing.Type = model.Type;
        existing.MinHeightInCM = model.MinHeightInCM;
        existing.Capacity = model.Capacity;
    }
    public static void Delete(int id) => Items.RemoveAll(a => a.ID == id);
}

public static class BookingAccess
{
    private static readonly List<BookingModel> Bookings =
    [
        new BookingModel
        {
            OrderNumber = "ORD-guest-20240101000000",
            BookingDate = "2024-01-01",
            OriginalPrice = "10",
            Discount = "0",
            FinalPrice = "10",
            SessionId = 1,
            Quantity = 1,
            CustomerId = 1
        }
    ];

    public static IEnumerable<BookingModel> GetByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Enumerable.Empty<BookingModel>();

        return Bookings.Where(b =>
            b.OrderNumber.Contains(username, StringComparison.OrdinalIgnoreCase));
    }
}

public static class ComplaintsAccess
{
    private static readonly List<ComplaintModel> Complaints = [];
    private static int _nextId = 1;

    public static int NextId() => _nextId++;

    public static void Write(ComplaintModel complaint)
    {
        Complaints.Add(complaint);
    }

    public static List<ComplaintModel> GetAll() => Complaints.ToList();

    public static List<ComplaintModel> Filter(string? category = null, string? username = null, string? status = null)
    {
        return Complaints
            .Where(c => category == null || c.Category == category)
            .Where(c => username == null || c.Username == username)
            .Where(c => status == null || c.Status == status)
            .ToList();
    }

    public static void UpdateStatus(int id, string status)
    {
        var complaint = Complaints.FirstOrDefault(c => c.Id == id);
        if (complaint != null) complaint.Status = status;
    }

    public static void Delete(int id) => Complaints.RemoveAll(c => c.Id == id);
}

public static class MenusAccess
{
    private static readonly List<MenuModel> MenuItems =
    [
        new MenuModel(1, "Burger", "Cola", 8),
        new MenuModel(2, "Fries", "", 3)
    ];

    public static IEnumerable<MenuModel> GetAll() => MenuItems;
    public static MenuModel? GetById(int id) => MenuItems.FirstOrDefault(m => m.ID == id);
    public static void Insert(MenuModel model)
    {
        model.ID = MenuItems.Max(m => m.ID) + 1;
        MenuItems.Add(model);
    }

    public static void Delete(int id) => MenuItems.RemoveAll(m => m.ID == id);
}

public static class UserAccess
{
    private static readonly List<UserModel> Users =
    [
        new UserModel(1, "Alice", "alice@mail", "01-01-2000", 160, "+31000000000", "Aa!12345")
    ];

    public static int NextId() => Users.Max(u => u.Id) + 1;

    public static void Write(UserModel account)
    {
        Users.Add(account);
    }

    public static UserModel? GetById(int id) => Users.FirstOrDefault(u => u.Id == id);

    public static void Update(UserModel account)
    {
        var existing = GetById(account.Id);
        if (existing == null) return;
        existing.Name = account.Name;
        existing.Email = account.Email;
        existing.DateOfBirth = account.DateOfBirth;
        existing.Height = account.Height;
        existing.Phone = account.Phone;
        existing.Password = account.Password;
    }
}

public static class ReservationAccess
{
    private static readonly List<ReservationModel> Bookings = [];

    public static void AddBooking(ReservationModel booking)
    {
        Bookings.Add(booking);
    }

    public static List<ReservationModel> GetAllBookings() => Bookings;
}

public static class SessionAccess
{
    private static readonly List<Session> Sessions =
    [
        new Session
        {
            Id = 1,
            Nr = "S1",
            Date = DateTime.Today.ToString("yyyy-MM-dd"),
            Time = "10:00",
            AttractionID = 1,
            BasisPrice = 15,
            MaxCapacity = 30,
            CurrentBookings = 0,
            IsActive = true
        }
    ];

    public static List<Session> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date)
        => Sessions.Where(s => s.AttractionID == attractionId).ToList();

    public static int GetCapacityBySession(Session session) => session.MaxCapacity;

    public static Session? GetSessionById(int id) => Sessions.FirstOrDefault(s => s.Id == id);

    public static List<Session> GetAllSessions() => Sessions;

    public static void UpdateSession(Session session)
    {
        var existing = GetSessionById(session.Id);
        if (existing == null) return;
        existing.CurrentBookings = session.CurrentBookings;
    }
}

public static class BirthdayTicketAccess
{
    private static readonly HashSet<(int userId, int year)> Usage = [];

    public static bool HasUsedBirthdayTicket(int userId, int year) => Usage.Contains((userId, year));

    public static void RecordBirthdayTicketUsage(int userId, int year, string orderNumber)
    {
        Usage.Add((userId, year));
    }
}

public static class GroupPaymentRepository
{
    private static readonly HashSet<string> PaidOrders = [];

    public static bool IsOrderPaid(string orderNumber) => PaidOrders.Contains(orderNumber);
    public static bool SimulateGateway() => true;
    public static void InsertGroupPayment(string orderNumber, decimal amount, string method, string confirmationNumber)
    {
        PaidOrders.Add(orderNumber);
    }
}

namespace MyProject.DAL
{
    public static class OfferAccess
    {
        private static readonly List<OfferBase> Offers =
        [
            new OfferGroup("GRP", "Group Fun", "Group", 0.2m, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(10), true, 0, 10, 2, GroupType.Company),
            new OfferVIP("VIP", "Vip Bonus", "Vip", 0.1m, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(5), true, 0),
            new OfferPromoCode("PR", "PROMO", DateTime.Today.AddDays(5), 10, 0, 0.15m)
        ];

        public static List<OfferBase> GetActiveOffers() => Offers;

        public static void IncrementPromoUse(int promoId) { }
    }

    public static class OfferManagementRepository
    {
        private static readonly List<OfferBase> Offers = OfferAccess.GetActiveOffers();

        public static List<OfferBase> GetAllOffers() => Offers;
        public static OfferBase? GetOfferById(int id) => Offers.FirstOrDefault(o => o.Id == id);
        public static int InsertOffer(OfferBase offer)
        {
            offer.Id = Offers.Count + 1;
            Offers.Add(offer);
            return offer.Id;
        }
        public static int UpdateOffer(OfferBase offer)
        {
            var existing = Offers.FirstOrDefault(o => o.Id == offer.Id);
            if (existing == null) return 0;
            Offers.Remove(existing);
            Offers.Add(offer);
            return 1;
        }
        public static int DeleteOffer(int id)
        {
            return Offers.RemoveAll(o => o.Id == id);
        }
        public static int ToggleActive(int id, bool isActive)
        {
            var offer = Offers.FirstOrDefault(o => o.Id == id);
            if (offer == null) return 0;
            offer.IsActive = isActive;
            return 1;
        }
    }

    public static class GroupReservationRepository
    {
        private static readonly List<GroupReservationDto> Reservations = [];
        private static readonly List<SessionAvailabilityDto> Sessions =
        [
            new SessionAvailabilityDto
            {
                Id = 1,
                Date = DateTime.Today.ToString("yyyy-MM-dd"),
                Time = "09:00",
                BasisPrice = 12,
                AvailableSeats = 100
            }
        ];

        public static List<SessionAvailabilityDto> GetAvailableSessions(int groupSize)
            => Sessions.Where(s => s.AvailableSeats >= groupSize).ToList();

        public static int InsertGroupOrder(GroupReservationDto dto)
        {
            dto.Id = Reservations.Count + 1;
            Reservations.Add(dto);
            return dto.Id;
        }

        public static int IncrementSessionBooking(int sessionId, int quantity)
        {
            var session = Sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null) return 0;
            session.AvailableSeats -= quantity;
            return 1;
        }

        public static string GenerateOrderNumber(string prefix)
            => $"{prefix}-GRP-{DateTime.Now:yyyyMMddHHmmss}";
    }

    public static class PaymentRepository
    {
        public static bool SimulateGateway(PaymentMethods method) => true;
        public static void InsertPayment(string orderNumber, decimal amount, string method, string confirmationNumber)
        {
        }
    }
}

