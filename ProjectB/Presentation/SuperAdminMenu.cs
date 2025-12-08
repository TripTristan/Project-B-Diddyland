using System;

public class SuperAdminMenu
{
    private readonly LoginStatus _loginStatus;
    private readonly UiHelpers _ui;
    private readonly AttractieMenu _attractieMenu;
    private readonly MenuForm _menuForm;
    private readonly OrderForm _orderForm;
    private readonly ReservationUI _reservationUI;
    private readonly ParkMap _parkMap;
    private readonly ManageAdmins _manageAdmins;
    private readonly AdminComplaintsPage _adminComplaintsPage;
    private readonly UserLogoutUI _logoutUi;

    public SuperAdminMenu(
        LoginStatus loginStatus,
        UiHelpers ui,
        AttractieMenu attractieMenu,
        MenuForm menuForm,
        OrderForm orderForm,
        ReservationUI reservationUI,
        ParkMap parkMap,
        ManageAdmins manageAdmins,
        AdminComplaintsPage adminComplaintsPage,
        UserLogoutUI logoutUi)
    {
        _loginStatus = loginStatus;
        _ui = ui;
        _attractieMenu = attractieMenu;
        _menuForm = menuForm;
        _orderForm = orderForm;
        _reservationUI = reservationUI;
        _parkMap = parkMap;
        _manageAdmins = manageAdmins;
        _adminComplaintsPage = adminComplaintsPage;
        _logoutUi = logoutUi;
    }

    public void Run()
    {
        while (_loginStatus.CurrentUserInfo != null &&
               _loginStatus.CurrentUserInfo.Role == (int)UserRole.SuperAdmin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Super Admin Dashboard");
            string Prompt = $"Logged in as: {LoginStatus.CurrentUserInfo.Username} (Super Admin)";
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"Financial Dashboard (NEW)"},
                new List<string> {"Attractions"},
                new List<string> {"Menu management"}, 
                new List<string> {"Orders"}, 
                new List<string> {"Reservations"},
                new List<string> {"Map"},
                new List<string> {"Manage Admins"},
                new List<string> {"Manage Complaints"},
                new List<string> {"Logout"},
                new List<string> {"Quit"},
            };

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    FinancialMenu.Start();
                    break;
                case 1:
                    AttractieMenu.Start();
                    break;

                case 2:
                    MenuForm.Run();
                    break;

                case 3:
                    OrderForm.Run();
                    break;

                case 4:
                    ReservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case 5:
                    ParkMap.ShowInteractive();
                    break;

                case 6:
                    ManageAdmins.Show();
                    break;

                case 7:
                    AdminComplaintsPage.Show();
                    break;

                case 8:
                    new UserLogoutUI().Start();
                    UiHelpers.Pause();
                    return;

                case 9:
                    Environment.Exit(0);
                    return;

                default:
                    break;
            }
        }
    }
}
