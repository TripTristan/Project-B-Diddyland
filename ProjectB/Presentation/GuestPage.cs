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
                new List<string> {"Map"}, 
                new List<string> {"Orders"}, 
                new List<string> {"Reservations"}, 
                new List<string> {"Fastpass"}, 
                new List<string> {"Profile"}, 
                new List<string> {"Customer Complaints"}, 
                new List<string> {"Logout"}
            };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    ParkMap.ShowInteractive();
                    break;

                case 1:
                    OrderForm.Run();
                    break;

                case 2:
                    ReservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case 3:
                    FastPassUI.Run(LoginStatus.CurrentUserInfo);
                    UiHelpers.Pause();
                    break;

                case 4:
                    ProfilePage.Show(LoginStatus.CurrentUserInfo.Id);
                    UiHelpers.Pause();
                    break;

                case 5:
                    CustomerHelpPage.Show();
                    UiHelpers.Pause();
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