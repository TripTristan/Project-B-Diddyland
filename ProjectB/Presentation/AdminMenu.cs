using System;

public class AdminMenu
{
    private readonly LoginStatus _loginStatus;
    private readonly UiHelpers _ui;
    private readonly AttractieMenu _attractieMenu;
    private readonly MenuForm _menuForm;
    private readonly OrderForm _orderForm;
    private readonly ReservationUI _reservationUI;
    private readonly ParkMap _parkMap;
    private readonly AdminComplaintsPage _adminComplaintsPage;
    private readonly UserLogoutUI _logoutUi;

    public AdminMenu(
        LoginStatus loginStatus,
        UiHelpers ui,
        AttractieMenu attractieMenu,
        MenuForm menuForm,
        OrderForm orderForm,
        ReservationUI reservationUI,
        ParkMap parkMap,
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
        _adminComplaintsPage = adminComplaintsPage;
        _logoutUi = logoutUi;
    }

    public void Run()
    {
        while (_loginStatus.CurrentUserInfo != null &&
               _loginStatus.CurrentUserInfo.Role == (int)UserRole.Admin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Admin Dashboard");
            string Prompt = $"Logged in as: {LoginStatus.CurrentUserInfo.Username} (Admin)";
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"Map"}, 
                new List<string> {"Attractions"},
                new List<string> {"Menu management"}, 
                new List<string> {"Orders"}, 
                new List<string> {"Reservations"}, 
                new List<string> {"Logout"}, 
                new List<string> {"Manage Complaints"}, 
                new List<string> {"Quit"}
            };
            List<List<string>> MapOptions = new List<List<string>> 
            {
                new List<string> {"Rotterdam"}, 
                new List<string> {"Amsterdam"}
            };

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();


            switch (selectedIndex[0])
            {
                case 0:
                    Console.Clear();
                    MainMenu Menu = new MainMenu(MapOptions, Prompt);
                    int[] selectedIndex = Menu.Run();
                    string location = MapOptions[selectedIndex][0] == "2" ? "Amsterdam" : "Rotterdam";

                    _parkMap.ShowInteractive(location);
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
                    new UserLogoutUI().Start();
                    UiHelpers.Pause();
                    return;

                case 6:
                    AdminComplaintsPage.Show();
                    UiHelpers.Pause();
                    break;


                case 7:
                    Environment.Exit(0);
                    return;

                default:
                    break;
            }
        }
    }
}
