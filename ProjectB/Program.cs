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

        var ui = new UiHelpers();

        var db = new DatabaseContext("Data Source=DataSources/diddyland.db");

        var userAccess = new UserAccess(db);
        var attractiesAccess = new AttractiesAccess(db);
        var sessionAccess = new SessionAccess(db, attractiesAccess);
        var reservationAccess = new ReservationAccess(db, userAccess);
        var menuAccess = new MenusAccess(db);
        var complaintsAccess = new ComplaintsAccess(db);
        var bookingAccess = new BookingAccess(db);
        var financialAccess = new FinancialAccess(db);

        var userLogic = new UserLogic(userAccess);
        var loginLogic = new LoginLogic(userAccess, loginStatus);
        var logoutLogic = new LogoutLogic(loginStatus);
        var updateLogic = new UserUpdateLogic(userAccess, userLogic);
        var bookingHistoryLogic = new BookingHistoryLogic(bookingAccess);
        var attractieLogic = new AttractieLogic(attractiesAccess);
        var reservationLogic = new ReservationLogic(sessionAccess, reservationAccess);
        var fastPassLogic = new FastPassLogic(sessionAccess, reservationLogic, reservationAccess, attractiesAccess);
        var menuLogic = new MenuLogic(menuAccess);
        var orderLogic = new OrderLogic(menuLogic);
        var complaintLogic = new ComplaintLogic(complaintsAccess);
        var financialLogic = new FinancialLogic(reservationAccess, userAccess);

        var registerUI = new UserRegister(userLogic);
        var loginUI = new UserLoginUI(loginLogic);
        var logoutUI = new UserLogoutUI(logoutLogic);

        var bookingHistoryUI = new BookingHistoryUI(bookingHistoryLogic, sessionAccess, attractiesAccess);
        var attractieMenu = new AttractieMenu(attractieLogic);
        var menuForm = new MenuForm(menuLogic);
        var orderForm = new OrderForm(orderLogic, menuForm);
        var profilePage = new ProfilePage(updateLogic);
        var parkMap = new ParkMap();
        var financialMenu = new FinancialMenu(financialLogic);
        var adminComplaintsPage = new AdminComplaintsPage(complaintLogic, ui);

        var fastPassUI = new FastPassUI(
            fastPassLogic,
            attractiesAccess,
            sessionAccess,
            ui
        );

        var reservationUI = new ReservationUI(
            reservationLogic,
            new PaymentUI(),
            loginUI,
            ui,
            sessionAccess,
            loginStatus,
            financialLogic
        );

        var manageAdmins = new ManageAdmins(userAccess);
        var customerHelpPage = new CustomerHelpPage(complaintLogic, loginStatus);

        var guestMenu = new GuestMenu(
            loginStatus,
            ui,
            attractieMenu,
            menuForm,
            orderForm,
            reservationUI,
            fastPassUI,
            profilePage,
            bookingHistoryUI,
            customerHelpPage,
            logoutUI,
            parkMap
        );

        var adminMenu = new AdminMenu(
            loginStatus,
            ui,
            attractieMenu,
            menuForm,
            orderForm,
            reservationUI,
            parkMap,
            adminComplaintsPage,
            logoutUI
        );

        var superAdminMenu = new SuperAdminMenu(
            loginStatus,
            ui,
            attractieMenu,
            menuForm,
            orderForm,
            reservationUI,
            parkMap,
            manageAdmins,
            adminComplaintsPage,
            logoutUI,
            financialMenu
        );

        var app = new Application(
            loginStatus,
            ui,
            loginUI,
            registerUI,
            guestMenu,
            adminMenu,
            superAdminMenu,
            logoutUI
        );

        app.Run();
    }
}

public class Application
{
    private readonly LoginStatus _loginStatus;
    private readonly UiHelpers _ui;
    private readonly UserLoginUI _loginUI;
    private readonly UserRegister _registerUI;
    private readonly GuestMenu _guestMenu;
    private readonly AdminMenu _adminMenu;
    private readonly SuperAdminMenu _superAdminMenu;
    private readonly UserLogoutUI _logoutUI;

    public Application(
        LoginStatus loginStatus,
        UiHelpers ui,
        UserLoginUI loginUI,
        UserRegister registerUI,
        GuestMenu guestMenu,
        AdminMenu adminMenu,
        SuperAdminMenu superAdminMenu,
        UserLogoutUI logoutUI)
    {
        _loginStatus = loginStatus;
        _ui = ui;
        _loginUI = loginUI;
        _registerUI = registerUI;
        _guestMenu = guestMenu;
        _adminMenu = adminMenu;
        _superAdminMenu = superAdminMenu;
        _logoutUI = logoutUI;
    }

    public void Run()
    {
        while (true)
        {
            try
            {
                if (_loginStatus.CurrentUserInfo == null)
                {
                    ShowSplash();
                }
                else
                {
                    int role = _loginStatus.CurrentUserInfo.Role;

                    if (role == 0)
                        _guestMenu.Run();
                    else if (role == 1)
                        _adminMenu.Run();
                    else if (role == 2)
                        _superAdminMenu.Run();
                    else
                    {
                        UiHelpers.Warn("Invalid role. Logging out...");
                        _logoutUI.Start();
                        UiHelpers.Pause();
                    }
                }
            }
            catch (Exception ex)
            {
                UiHelpers.Error(ex.Message);
                UiHelpers.Pause();
            }
        }
    }
        private void ShowSplash()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        string Prompt = (@"
________  .___________  ________ _____.___.____       _____    _______  ________   
\______ \ |   \______ \ \______ \\__  |   |    |     /  _  \   \      \ \______ \  
 |    |  \|   ||    |  \ |    |  \/   |   |    |    /  /_\  \  /   |   \ |    |  \ 
 |    `   \   ||    `   \|    `   \____   |    |___/    |    \/    |    \|    `   \
/_______  /___/_______  /_______  / ______|_______ \____|__  /\____|__  /_______  /
        \/            \/        \/\/              \/       \/         \/        \/ 

===================================================================================");

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Login"},
            new List<string> {"Register"}, 
            new List<string> {"Continue as Guest"}, 
            new List<string> {"Quit"},
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        
        Console.ResetColor();

        switch (selectedIndex[0])
        {
            case 0: 
                _loginUI.StartLogin();
                UiHelpers.Pause();
                break;

            case 1: 
                _registerUI.Register();
                UiHelpers.Pause();
                break;

            case 2: 
                EnsureGuestSession();
                _guestMenu.Run();
                break;

            case 3: 
                return;
            

            default:
                Console.WriteLine("");
                break;
        }
 
        }

    private void EnsureGuestSession()
    {
        _loginStatus.Login(new UserModel
        {
            Id = 0,
            Name = "Guest",
            Email = "guest@local",
            Password = "",
            Phone = "",
            Height = 0,
            DateOfBirth = "",
            Admin = 0
        });
    }
}
