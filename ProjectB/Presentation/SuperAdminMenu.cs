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
            _ui.WriteHeader("Diddyland – Super Admin Dashboard");
            Console.WriteLine($"Logged in as: {_loginStatus.CurrentUserInfo.Username} (Super Admin)");
            Console.WriteLine();

            Console.WriteLine("1) Attractions");
            Console.WriteLine("2) Menu management");
            Console.WriteLine("3) Orders");
            Console.WriteLine("4) Reservations");
            Console.WriteLine("5) Map");
            Console.WriteLine("6) Manage Admins");
            Console.WriteLine("7) Manage Complaints");
            Console.WriteLine("8) Logout");
            Console.WriteLine("0) Quit\n");

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    _attractieMenu.Start();
                    break;

                case "2":
                    _menuForm.Run();
                    break;

                case "3":
                    _orderForm.Run();
                    break;

                case "4":
                    _reservationUI.StartReservation();
                    _ui.Pause();
                    break;

                case "5":
                    Console.Clear();
                    Console.WriteLine("Select park location:");
                    Console.WriteLine("1) Diddyland Rotterdam");
                    Console.WriteLine("2) Diddyland Amsterdam");
                    Console.Write("\nEnter choice: ");
                    string? input = Console.ReadLine()?.Trim();
                    string location = input == "2" ? "Amsterdam" : "Rotterdam";
                    _parkMap.ShowInteractive(location);
                    break;

                case "6":
                    _manageAdmins.Show();
                    break;

                case "7":
                    _adminComplaintsPage.Show();
                    _ui.Pause();
                    break;

                case "8":
                    _logoutUi.Start();
                    _ui.Pause();
                    return;

                case "0":
                    Environment.Exit(0);
                    return;

                default:
                    _ui.Warn("Unknown option.");
                    _ui.Pause();
                    break;
            }
        }
    }
}
