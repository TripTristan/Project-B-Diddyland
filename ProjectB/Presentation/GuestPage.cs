public class GuestMenu
{
    private readonly LoginStatus _loginStatus;
    private readonly UiHelpers _ui;
    private readonly AttractieMenu _attractieMenu;
    private readonly MenuForm _menuForm;
    private readonly OrderForm _orderForm;
    private readonly ReservationUI _reservationUI;
    private readonly FastPassUI _fastPassUI;
    private readonly ProfilePage _profilePage;
    private readonly BookingHistoryUI _bookingHistoryUI;
    private readonly CustomerHelpPage _customerHelpPage;
    private readonly UserLogoutUI _logoutUI;
    private readonly ParkMap _parkMap;

    public GuestMenu(
        LoginStatus loginStatus,
        UiHelpers ui,
        AttractieMenu attractieMenu,
        MenuForm menuForm,
        OrderForm orderForm,
        ReservationUI reservationUI,
        FastPassUI fastPassUI,
        ProfilePage profilePage,
        BookingHistoryUI bookingHistoryUI,
        CustomerHelpPage customerHelpPage,
        UserLogoutUI logoutUI,
        ParkMap parkMap)
    {
        _loginStatus = loginStatus;
        _ui = ui;
        _attractieMenu = attractieMenu;
        _menuForm = menuForm;
        _orderForm = orderForm;
        _reservationUI = reservationUI;
        _fastPassUI = fastPassUI;
        _profilePage = profilePage;
        _bookingHistoryUI = bookingHistoryUI;
        _customerHelpPage = customerHelpPage;
        _logoutUI = logoutUI;
        _parkMap = parkMap;
    }

    public void Run()
    {
        while (_loginStatus.CurrentUserInfo != null &&
               _loginStatus.CurrentUserInfo.Role == 0)
        {
            string Prompt = $"Diddyland – Guest Page\nLogged in as: {LoginStatus.CurrentUserInfo.Username} (Guest)";
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"Make Reservation"}, 
                new List<string> {"Buy Fastpass"}, 
                new List<string> {"Submit A Complaint"}, 
                new List<string> {"View Map"}, 
                new List<string> {"View Orders"}, 
                new List<string> {"View Profile"}, 
                new List<string> {"Logout"}
            };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    ReservationUI.StartReservation();
                    break;

                case 1:
                    FastPassUI.Run(LoginStatus.CurrentUserInfo);
                    break;

                case 2:
                    CustomerHelpPage.Show();
                    UiHelpers.Pause();
                    break;

                case 3:
                    ParkMap.ShowInteractive();
                    UiHelpers.Pause();
                    break;

                case 4:
                    OrderForm.Run();
                    UiHelpers.Pause();
                    break;

                case 5:
                    ProfilePage.Show(LoginStatus.CurrentUserInfo.Id);
                    break;

                case 6:
                    new UserLogoutUI().Start();
                    UiHelpers.Pause();
                    return;

                default:
                    _ui.Warn("Unknown option.");
                    _ui.Pause();
                    break;
            }
        }
    }
}
