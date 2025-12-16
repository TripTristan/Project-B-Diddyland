using System;

public class AdminMenu : AdminElements
{
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

                    _parkMap.ShowInteractive(location);
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
                    _reservationUI.StartReservation();
                    UiHelpers.Pause();
                    break;

                case 5:
                    _logoutUi.Start();
                    UiHelpers.Pause();
                    return;

                case 6:
                    _adminComplaintsPage.Show();
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
