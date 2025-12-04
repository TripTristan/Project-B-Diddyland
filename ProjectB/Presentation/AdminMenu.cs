using System;

static class AdminMenu
{
    public static void Run()
    {
        while (LoginStatus.CurrentUserInfo != null &&
               LoginStatus.CurrentUserInfo.Role == (int)UserRole.Admin)
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

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();


            switch (selectedIndex[0])
            {
                case 0:
                    ParkMap.ShowInteractive();
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
