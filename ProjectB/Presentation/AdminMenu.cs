static class AdminMenu
{
    public static void Run()
    {
        while (LoginStatus.CurrentUserInfo != null &&
              (LoginStatus.CurrentUserInfo.Role == 1 ||       // Admin 
               LoginStatus.CurrentUserInfo.Role == 2 ))      // SuperAdmin 
        {
            Console.Clear();
            var roleLabel = LoginStatus.CurrentUserInfo.Role == 2 ? "Super Admin" : "Admin";
            UiHelpers.WriteHeader($"Diddyland – {roleLabel} Dashboard");
            Console.WriteLine($"Logged in as: {LoginStatus.CurrentUserInfo.Username} ({roleLabel})");

            Console.WriteLine("1) Attractions");
            Console.WriteLine("2) Menu management");
            Console.WriteLine("3) Orders");
            Console.WriteLine("4) Reservations");
            Console.WriteLine("5) Map");
            Console.WriteLine("6) Logout");
            Console.WriteLine("0) Quit");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    AttractieMenu.Start();
                    break;

                case "2":
                    MenuForm.Run();
                    break;

                case "3":
                    OrderForm.Run();
                    break;

                case "4":
                    ReservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case "5":
                    ParkMap.ShowInteractive();
                    break;

                case "6":
                    new UserLogoutUI().Start();
                    UiHelpers.Pause();
                    return; 
    
                case "0":
                    Environment.Exit(0);
                    return;

                default:
                    UiHelpers.Warn("Unknown option.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }
}