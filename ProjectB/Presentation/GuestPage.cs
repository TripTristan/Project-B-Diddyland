static class GuestMenu
{
    public static void Run()
    {
        while (LoginStatus.CurrentUserInfo != null &&
               LoginStatus.CurrentUserInfo.Role == 0)
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
                    UiHelpers.Warn("Unknown option.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }
}