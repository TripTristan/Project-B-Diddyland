using System;

enum UserRole
{
    User = 0,
    Admin = 1,
    SuperAdmin = 2
}

partial class Program
{
    static void Main()
    {
        var loginStatus = new LoginStatus();
        loginStatus.Logout();

        var db = new DatabaseContext("Data Source=DataSources/diddyland.db");

        var userAccess = new UserAccess(db);
        var attractiesAccess = new AttractiesAccess(db);
        var sessionAccess = new SessionAccess(db, attractiesAccess);
        var reservationAccess = new ReservationAccess(db, userAccess);
        var menuAccess = new MenusAccess(db);
        var complaintsAccess = new ComplaintsAccess(db);
        var bookingAccess = new BookingAccess(db);
        var financialAccess = new FinancialAccess(db);
        var discountAccess = new DiscountCodeAccess(db);

        var userLogic = new UserLogic(userAccess);
        var authLogic = new AuthenticationLogic(loginStatus);
        var bookingHistoryLogic = new BookingHistoryLogic(bookingAccess);
        var attractionLogic = new AttractionLogic(attractiesAccess);
        var reservationLogic = new ReservationLogic(sessionAccess, reservationAccess);
        var fastPassLogic = new FastPassLogic(sessionAccess, reservationLogic, reservationAccess, attractiesAccess);
        var foodmenuLogic = new FoodmenuLogic();
        var complaintLogic = new ComplaintLogic(complaintsAccess);
        var financialLogic = new FinancialLogic(reservationAccess, userAccess);
        var discountLogic = new DiscountCodeLogic(discountAccess);

        var adminContext = new AdminContext(loginStatus, attractionLogic, foodmenuLogic, financialLogic, complaintLogic, discountLogic, adminSuper);
        var userContext = new UserContext(loginStatus, reservationLogic, fastPassLogic, bookingHistoryLogic, userLogic, updateLogic, foodmenuLogic, complaintLogic, authLogic, discountLogic);

        var adminManagement = new ManageAdmins(adminContext);
        var adminAttraction = new AdminAttraction(adminContext);
        var adminFoodmenu = new AdminFoodmenu(adminContext);
        var adminFinance = new AdminFinance(adminContext);
        var discount = new DiscountCode(adminContext);
        var adminComplaints = new AdminComplaints(adminContext);

        var profile = new Profile(userContext);
        var register = new UserRegister(userLogic);
        var userAuth = new UserAuthentication(userContext);
        var userHelp = new UserHelp(userContext);
        var userGuest = new UserGuest(userContext);
        var reservation = new UserReservation(userContext);
        var userFoodmenu = new UserFoodmenu(userContext);
        var bookingHistory = new BookingHistory(userContext);



        var app = new Application(
            loginStatus,
            userAuth,
            userGuest,
            admin,
            adminSuper,
            register
        );

        app.Run();
    }
}