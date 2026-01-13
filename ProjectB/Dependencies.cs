public class Dependencies
{
    private DatabaseContext db;

    // Status
    public LoginStatus loginStatus;

    // Access layers
    public UserAccess userAccess;
    public AttractiesAccess attractiesAccess;
    public SessionAccess sessionAccess;
    public ReservationAccess reservationAccess;
    public MenusAccess menuAccess;
    public ComplaintsAccess complaintsAccess;
    public BookingAccess bookingAccess;
    public FinancialAccess financialAccess;
    public DiscountCodeAccess discountAccess;

    // Logic layers
    public UserLogic userLogic;
    public AuthenticationLogic authLogic;
    public BookingHistoryLogic bookingHistoryLogic;
    public AttractionLogic attractionLogic;
    public ReservationLogic reservationLogic;
    public FastPassLogic fastPassLogic;
    public FoodmenuLogic foodmenuLogic;
    public ComplaintLogic complaintLogic;
    public FinancialLogic financialLogic;
    public DiscountLogic discountLogic;
    public LoyaltyDiscountLogic loyaltyDiscountLogic;

    // UI / Controllers
    public ManageAdmins adminManagement;
    public AdminAttraction adminAttraction;
    public AdminFoodmenu adminFoodmenu;
    public AdminFinance adminFinance;
    public DiscountCode discount;
    public AdminComplaints adminComplaints;
    public Profile profile;
    public UserRegister register;
    public UserAuthentication userAuth;
    public UserHelp userHelp;
    public UserGuest userGuest;
    public UserReservation reservation;
    public UserFoodmenu userFoodmenu;
    public BookingHistory bookingHistory;
    public FastPass fastPass;
    public Admin admin;
    public AdminSuper adminSuper;
    public Application app;

    public Dependencies(DatabaseContext dB)
    {
        // Core
        db = dB;
        loginStatus = new LoginStatus();

        // Access
        userAccess = new UserAccess(db);
        attractiesAccess = new AttractiesAccess(db);
        sessionAccess = new SessionAccess(db, attractiesAccess);
        reservationAccess = new ReservationAccess(db, userAccess);
        menuAccess = new MenusAccess(db);
        complaintsAccess = new ComplaintsAccess(db);
        bookingAccess = new BookingAccess(db);
        financialAccess = new FinancialAccess(db);
        discountAccess = new DiscountCodeAccess(db);

        // Logic
        userLogic = new UserLogic(userAccess);
        authLogic = new AuthenticationLogic(loginStatus, userAccess);
        bookingHistoryLogic = new BookingHistoryLogic(this);
        attractionLogic = new AttractionLogic(attractiesAccess);
        reservationLogic = new ReservationLogic(this);
        fastPassLogic = new FastPassLogic(
            sessionAccess,
            reservationLogic,
            reservationAccess,
            attractiesAccess
        );
        foodmenuLogic = new FoodmenuLogic();
        complaintLogic = new ComplaintLogic(complaintsAccess);
        financialLogic = new FinancialLogic(reservationAccess, userAccess);
        discountLogic = new DiscountLogic(discountAccess);
        loyaltyDiscountLogic = new LoyaltyDiscountLogic(reservationAccess, userAccess);

        adminManagement = new ManageAdmins(this);
        adminAttraction = new AdminAttraction(this);
        adminFoodmenu = new AdminFoodmenu(this);
        adminFinance = new AdminFinance(this);
        discount = new DiscountCode(this);
        adminComplaints = new AdminComplaints(this);
        profile = new Profile(this);
        register = new UserRegister(userLogic);
        userAuth = new UserAuthentication(this);
        userHelp = new UserHelp(this);
        userGuest = new UserGuest(this);
        reservation = new UserReservation(this);
        userFoodmenu = new UserFoodmenu(this);
        bookingHistory = new BookingHistory(bookingHistoryLogic);
        fastPass = new FastPass(this);
        admin = new Admin(this);
        adminSuper = new AdminSuper(this);


        app = new Application(
            loginStatus,
            userAuth,
            userGuest,
            register,
            admin,
            adminSuper
        );
    }

    public UserModel SelectUser()
    {
        List<UserModel> users = financialLogic.GrabAllUsers();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            UiHelpers.Pause();
            return null;
        }

        List<List<string>> Options = new List<List<string>>();

        for (int i = 0; i < users.Count; i++)
        {
            List<string> temp = new List<string>();
            temp.Add(users[i].Name);
            Options.Add(temp);
        }

        MainMenu menu = new MainMenu(Options, "Select a user to view revenue:");
        int[] selected = menu.Run();

        return users[selected[0]];
    }
}
