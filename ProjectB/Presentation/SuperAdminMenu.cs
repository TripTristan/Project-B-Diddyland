using System;

public class SuperAdminMenu : AdminElements

{
    public void Run()
    {
        while (_loginStatus.CurrentUserInfo != null &&
               _loginStatus.CurrentUserInfo.Role == (int)UserRole.SuperAdmin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Super Admin Dashboard");
            string Prompt = $"Logged in as: {_loginStatus.CurrentUserInfo.Username} (Super Admin)";
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"Financial Dashboard (NEW)"},
                new List<string> {"Attractions"},
                new List<string> {"Menu management"}, 
                new List<string> {"Orders"}, 
                new List<string> {"Reservations"},
                new List<string> {"Map"},
                new List<string> {"Manage Admins"},
                new List<string> {"Manage Complaints"},
                new List<string> {"Create Discount Code"},
                new List<string> {"Logout"},
                new List<string> {"Quit"},
            };

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    _financialMenu.Start();
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
                    _reservationManagementUI.Start();
                    UiHelpers.Pause();
                    break;

                case 5:
                    _parkMap.ShowInteractive("rotterdam");
                    break;

                case 6:
                    _manageAdmins.Show();
                    break;

                case 7:
                    _adminComplaintsPage.Show();
                    break;
                
                case 8:
                    _discountCodeUI.ShowCreate();
                    break;
                
                case 9:
                    _logoutUi.Start();
                    UiHelpers.Pause();
                    return;

                case 10:
                    Environment.Exit(0);
                    return;

                default:
                    break;
            }
        }
    }
}
