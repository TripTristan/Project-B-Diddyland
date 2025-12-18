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
    private readonly AdminReservationUI _adminReservationUI;
    private readonly AdminTimeslotUI _adminTimeslotUI;


    public AdminMenu(
        LoginStatus loginStatus,
        UiHelpers ui,
        AttractieMenu attractieMenu,
        MenuForm menuForm,
        OrderForm orderForm,
        ReservationUI reservationUI,
        ParkMap parkMap,
        AdminComplaintsPage adminComplaintsPage,
        UserLogoutUI logoutUi,
        AdminReservationUI adminReservationUI,
        AdminTimeslotUI adminTimeslotUI)
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
        _adminReservationUI = adminReservationUI;
        _adminTimeslotUI = adminTimeslotUI;
    }

    public void Run()
    {
        while (_loginStatus.CurrentUserInfo != null &&
               _loginStatus.CurrentUserInfo.Role == (int)UserRole.Admin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Admin Dashboard");
            string Prompt = $"Logged in as: {_loginStatus.CurrentUserInfo.Username} (Admin)";
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"Map"}, 
                new List<string> {"Attractions"},
                new List<string> {"Menu management"}, 
                new List<string> {"Orders"}, 
                new List<string> {"Manage Reservations"},
                new List<string> {"Manage Timeslots"},
                new List<string> {"Book a reservation"}, 
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
                    MainMenu MapMenu = new MainMenu(MapOptions, Prompt);
                    selectedIndex = MapMenu.Run();
                    string location = MapOptions[selectedIndex[0]][0];

                    _parkMap.ShowMap();
                    break;
                case 1:
                    _attractieMenu.Start();
                    break;

                case 2:
                    _menuForm.Run();
                    break;

                case 3:
                    _orderForm.Run();
                    break;

                case 4:
                    _adminReservationUI.Run();
                    UiHelpers.Pause();
                    break;
                
                case 5:
                    _adminTimeslotUI.Run();
                    UiHelpers.Pause();
                    break;

                case 6:
                    _reservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case 7:
                    _logoutUi.Start();
                    UiHelpers.Pause();
                    return;

                case 8:
                    _adminComplaintsPage.Show();
                    UiHelpers.Pause();
                    break;


                case 9:
                    Environment.Exit(0);
                    return;

                default:
                    break;
            }
        }
    }
}
