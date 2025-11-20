using MyProject.BLL;

namespace ProjectB.Tests;

[TestClass]
public sealed class AttractieLogicTests
{
    [TestMethod]
    public void GetAll_ReturnsCollection()
    {
        var result = AttractieLogic.GetAll();
        Assert.IsNotNull(result);
    }
}

[TestClass]
public sealed class BookingHistoryLogicTests
{
    [TestMethod]
    public void GetUserBookingsRaw_WithBlankUsername_ReturnsEmpty()
    {
        var rows = BookingHistoryLogic.GetUserBookingsRaw("");
        Assert.AreEqual(0, rows.Count);
    }

    [TestMethod]
    public void GetUserBookingsGroupedByYearMonth_ReturnsNullWhenNoData()
    {
        var groups = BookingHistoryLogic.GetUserBookingsGroupedByYearMonth("missing-user");
        Assert.IsNull(groups);
    }
}

[TestClass]
public sealed class CartLineTests
{
    [TestMethod]
    public void LineTotal_ReflectsQuantityAndPrice()
    {
        var menu = new MenuModel(1, "Burger", "Cola", 5.5);
        var cartLine = new CartLine(menu, 2);

        Assert.AreEqual(11.0, cartLine.LineTotal, 0.001);
    }

    [TestMethod]
    public void OrderLineSnapshot_GeneratesLabel()
    {
        var snapshot = new OrderLineSnapshot(1, "Hotdog", "Soda", 4.0, 3);
        Assert.IsTrue(snapshot.Label.Contains("Hotdog"));
        Assert.AreEqual(12.0, snapshot.Subtotal, 0.001);
    }
}

[TestClass]
public sealed class ComplaintLogicTests
{
    [TestMethod]
    public void FilterComplaints_WithoutCriteria_ReturnsList()
    {
        var complaints = ComplaintLogic.FilterComplaints();
        Assert.IsNotNull(complaints);
    }
}

[TestClass]
public sealed class DiscountLogicTests
{
    [TestMethod]
    public void ApplyAllDiscounts_WithSimpleCart_ReturnsSameCount()
    {
        var cart = new List<(int sessionId, int age)>
        {
            (1, 10),
            (2, 35),
        };

        var summary = DiscountLogic.ApplyAllDiscounts(cart, null);

        Assert.AreEqual(cart.Count, summary.TicketDetails.Count);
        Assert.IsTrue(summary.OriginalSubTotal >= summary.FinalTotal);
    }
}

[TestClass]
public sealed class FastPassLogicTests
{
    [TestMethod]
    public void GetAvailableSessions_DoesNotThrow()
    {
        var sessions = FastPassLogic.GetAvailableFastPassSessions(1, DateTime.Today);
        Assert.IsNotNull(sessions);
    }
}

[TestClass]
public sealed class GroupPaymentLogicTests
{
    [TestMethod]
    public void ProcessGroupPayment_ReturnsResult()
    {
        var result = GroupPaymentLogic.ProcessGroupPayment(
            $"ORDER-{Guid.NewGuid():N}",
            25.0m,
            PaymentMethods.PayPal);

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
    }
}

[TestClass]
public sealed class GroupReservationLogicTests
{
    [TestMethod]
    public void ValidateGroupTypeChoice_WithValidInput()
    {
        var (ok, type) = GroupReservationLogic.ValidateGroupTypeChoice("1");
        Assert.IsTrue(ok);
        Assert.AreEqual(GroupType.School, type);
    }

    [TestMethod]
    public void ValidateGroupTypeChoice_WithInvalidInput()
    {
        var (ok, type) = GroupReservationLogic.ValidateGroupTypeChoice("abc");
        Assert.IsFalse(ok);
        Assert.AreEqual(GroupType.None, type);
    }
}

[TestClass]
public sealed class LoginInterfaceTests
{
    [TestMethod]
    public void CustomLoginLogic_CompliesWithInterface()
    {
        ILoginLogic logic = new SampleLoginLogic();
        var user = logic.Authenticate("demo", "demo");
        Assert.IsNotNull(user);
        Assert.AreEqual("demo", user.Account);
    }

    private sealed class SampleLoginLogic : ILoginLogic
    {
        public User? Authenticate(string account, string password)
            => new User("nr", "demo", "phone", "mail", "01-01-2000", "addr", account, password);
    }
}

[TestClass]
public sealed class LogoutInterfaceTests
{
    [TestMethod]
    public void CustomLogoutLogic_CompliesWithInterface()
    {
        var recorder = new Recorder();
        ILogoutLogic logic = new SampleLogoutLogic(recorder);
        logic.Logout();
        Assert.IsTrue(recorder.Called);
    }

    private sealed class Recorder
    {
        public bool Called { get; set; }
    }

    private sealed class SampleLogoutLogic : ILogoutLogic
    {
        private readonly Recorder _recorder;

        public SampleLogoutLogic(Recorder recorder) => _recorder = recorder;

        public void Logout() => _recorder.Called = true;
    }
}

[TestClass]
public sealed class LoginLogicTests
{
    [TestMethod]
    public void Authenticate_ReturnsNull_WhenPasswordMismatch()
    {
        var repo = new FakeUserRepository();
        var session = new FakeSessionService();
        var sut = new LoginLogic(repo, session);

        Assert.IsNull(sut.Authenticate("bob", "wrong"));
        Assert.IsNull(session.CurrentUser);
    }

    [TestMethod]
    public void Authenticate_ReturnsUser_WhenPasswordMatches()
    {
        var repo = new FakeUserRepository();
        var session = new FakeSessionService();
        var sut = new LoginLogic(repo, session);

        var user = sut.Authenticate("bob", "secret");

        Assert.IsNotNull(user);
        Assert.AreEqual(user, session.CurrentUser);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User _user = new(
            "nr",
            "Bob",
            "000",
            "bob@mail",
            "01-01-1990",
            "addr",
            "bob",
            "secret");

        public User? GetByAccount(string account)
            => account == "bob" ? _user : null;
    }

    private sealed class FakeSessionService : ISessionService
    {
        public User? CurrentUser { get; private set; }

        public void ClearCurrentUser() => CurrentUser = null;

        public void SetCurrentUser(User user) => CurrentUser = user;
    }
}

[TestClass]
public sealed class LoginStatusTests
{
    [TestMethod]
    public void LoginAndLogout_TurnsCurrentUserOnAndOff()
    {
        var user = new UserModel(1, "Alice", "mail", "01-01-2000", 150, "000", "pwd");
        LoginStatus.Login(user);
        Assert.AreEqual(user, LoginStatus.CurrentUser);

        LoginStatus.Logout();
        Assert.IsNull(LoginStatus.CurrentUser);
    }
}

[TestClass]
public sealed class LogoutLogicTests
{
    [TestMethod]
    public void Logout_CallsSessionService()
    {
        var session = new RecordingSessionService();
        var sut = new LogoutLogic(session);

        sut.Logout();

        Assert.IsNull(session.CurrentUser);
    }

    private sealed class RecordingSessionService : ISessionService
    {
        public User? CurrentUser { get; private set; }

        public void ClearCurrentUser() => CurrentUser = null;

        public void SetCurrentUser(User user) => CurrentUser = user;
    }
}

[TestClass]
public sealed class MenuLogicTests
{
    [TestMethod]
    public void AddItem_WithNoNames_ReturnsValidationMessage()
    {
        var message = MenuLogic.AddItem(null, null, 5);
        StringAssert.Contains(message, "Please provide");
    }

    [TestMethod]
    public void AddItem_WithNegativePrice_ReturnsValidationMessage()
    {
        var message = MenuLogic.AddItem("Food", null, -1);
        StringAssert.Contains(message, "Price cannot be negative");
    }
}

[TestClass]
public sealed class OfferManagementLogicTests
{
    [TestMethod]
    public void AddOffer_WithMissingName_FailsValidation()
    {
        var dto = new OfferDto
        {
            Nr = "O-1",
            Discount = 0.2m,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Type = OfferType.Age,
            Min = 0,
            Max = 10,
            IsActive = true,
        };

        var result = OfferManagementLogic.AddOffer(dto);

        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains(result.Message ?? string.Empty, "Name is required");
    }
}

[TestClass]
public sealed class OrderLogicTests
{
    [TestMethod]
    public void CartStartsEmpty()
    {
        Assert.IsTrue(OrderLogic.IsCartEmpty());
        Assert.AreEqual(0, OrderLogic.GetCart().Count);
    }
}

[TestClass]
public sealed class PaymentLogicTests
{
    [TestMethod]
    public void ProcessPayment_ReturnsResult()
    {
        var result = PaymentLogic.ProcessPayment(
            $"ORDER-{Guid.NewGuid():N}",
            10m,
            PaymentMethods.CreditDebitCard);

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
    }
}

[TestClass]
public sealed class ReportLogicTests
{
    [TestMethod]
    public void GetOrderReport_WithNoData_ReturnsEmptyList()
    {
        var report = ReportLogic.GetOrderReport(DateTime.Now.Year, DateTime.Now.Month, 0);
        Assert.IsNotNull(report);
        Assert.AreEqual(0, report.Count);
    }
}

[TestClass]
public sealed class ReservationLogicTests
{
    [TestMethod]
    public void CalculateDiscountedPrice_ForChild_HasHalfPrice()
    {
        var (discount, finalPrice) = ReservationLogic.CalculateDiscountedPrice(20m, 10);
        Assert.AreEqual(0.5m, discount);
        Assert.AreEqual(10m, finalPrice);
    }

    [TestMethod]
    public void CalculateTotalPrice_SumsTickets()
    {
        var total = ReservationLogic.CalculateTotalPrice(new List<(int age, int qty)>
        {
            (10, 2),
            (30, 1),
        });

        Assert.IsTrue(total > 0);
    }

    [TestMethod]
    public void GenerateOrderNumber_ReturnsFormattedValue()
    {
        var order = ReservationLogic.GenerateOrderNumber(null);
        StringAssert.StartsWith(order, "ORD-GUEST-");
    }
}

[TestClass]
public sealed class UserLogicTests
{
    [TestMethod]
    public void IsPhoneValid_RecognizesInternationalFormat()
    {
        Assert.IsTrue(UserLogic.IsPhoneValid("+12345678901"));
        Assert.IsFalse(UserLogic.IsPhoneValid("123456"));
    }

    [TestMethod]
    public void IsEmailValid_RequiresAtSymbol()
    {
        Assert.IsTrue(UserLogic.IsEmailValid("demo@example.com"));
        Assert.IsFalse(UserLogic.IsEmailValid("demo.example.com"));
    }

    [TestMethod]
    public void IsPasswordValid_EnforcesRules()
    {
        Assert.IsTrue(UserLogic.IsPasswordValid("Aa!12345"));
        Assert.IsFalse(UserLogic.IsPasswordValid("short"));
    }

    [TestMethod]
    public void IsDateOfBirthValid_InvalidDayFails()
    {
        Assert.IsFalse(UserLogic.IsDateOfBirthValid("31-02-2020"));
    }
}

[TestClass]
public sealed class UserUpdateLogicTests
{
    [TestMethod]
    public void UpdateProfile_InvalidEmail_FailsValidation()
    {
        var logic = new UserUpdateLogic();
        var model = new UserModel
        {
            Id = 1,
            Name = "Test User",
            Email = "invalid",
            DateOfBirth = "01-01-2000",
            Height = 180,
            Phone = "+12345678901",
            Password = "Aa!12345"
        };

        var (ok, error) = logic.UpdateProfile(model);

        Assert.IsFalse(ok);
        StringAssert.Contains(error ?? string.Empty, "Invalid email");
    }
}
