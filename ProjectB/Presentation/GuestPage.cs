static class GuestMenu
{
    public static void Run()
    {
        while (LoginStatus.CurrentUserInfo != null &&
               LoginStatus.CurrentUserInfo.Role == 0)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Guest Page");
            Console.WriteLine($"Logged in as: {LoginStatus.CurrentUserInfo.Username} (Guest)");
            Console.WriteLine("1) Map");
            Console.WriteLine("2) Orders");
            Console.WriteLine("3) Reservations");
            Console.WriteLine("4) Fastpass");
            Console.WriteLine("5) Profile");
            Console.WriteLine("6) Booking History");
            Console.WriteLine("7) Customer Complaints");
            Console.WriteLine("8) Logout");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ParkMap.ShowInteractive();
                    break;

                case "2":
                    OrderForm.Run();
                    break;

                case "3":
                    ReservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case "4":
                    FastPassUI.Run(LoginStatus.CurrentUserInfo);
                    UiHelpers.Pause();
                    break;

                case "5":
                    ProfilePage.Show(LoginStatus.CurrentUserInfo.Id);
                    UiHelpers.Pause();
                    break;
                
                case "6": 
                    BookingHistoryUI.Display(LoginStatus.CurrentUserInfo.Username);
                    UiHelpers.Pause();
                    break;
                
                case "7":
                    CustomerHelpPage.Show();
                    UiHelpers.Pause();
                    break;

                case "8":
                    new UserLogoutUI().Start();
                    UiHelpers.Pause();
                    return;

                default:
                    UiHelpers.Warn("Unknown option.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }
}