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
            Console.Clear();
            _ui.WriteHeader("Diddyland – Guest Page");
            Console.WriteLine($"Logged in as: {_loginStatus.CurrentUserInfo.Name} (Guest)");

            Console.WriteLine("1) Map");
            Console.WriteLine("2) Orders");
            Console.WriteLine("3) Reservations");
            Console.WriteLine("4) Fastpass");
            Console.WriteLine("5) Profile");
            Console.WriteLine("6) Booking History");
            Console.WriteLine("7) Customer Complaints");
            Console.WriteLine("8) Inbox");
            Console.WriteLine("9) Logout");
            Console.Write("\nChoose: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    _parkMap.ShowMap();
                    break;

                case "2":
                    _orderForm.Run();
                    break;

                case "3":
                    _reservationUI.StartReservation();
                    _ui.Pause();
                    break;

                case "4":
                    _fastPassUI.Run(_loginStatus.CurrentUserInfo);
                    _ui.Pause();
                    break;

                case "5":
                    _profilePage.Show(_loginStatus.CurrentUserInfo.Id);
                    _ui.Pause();
                    break;

                case "6":
                    _bookingHistoryUI.Display(_loginStatus.CurrentUserInfo.Name);
                    _ui.Pause();
                    break;

                case "7":
                    _customerHelpPage.Show();
                    _ui.Pause();
                    break;

                case "8":
                    _customerHelpPage.ShowHandledMessages();
                    _ui.Pause();
                    break;

                case "9":
                    _logoutUI.Start();
                    _ui.Pause();
                    return;

                default:
                    _ui.Warn("Unknown option.");
                    _ui.Pause();
                    break;
            }
        }
    }
}
