using System;

public class Admin
{
    private readonly AdminContext _ctx;

    private readonly AdminAttraction _adminAttraction;
    private readonly AdminFoodmenu _adminFoodmenu;
    private readonly AdminComplaints _adminComplaints;

    public Admin(AdminContext ctx)
    {
        _ctx = ctx;
        _adminAttraction = new AdminAttraction(ctx);
        _adminFoodmenu = new AdminFoodmenu(ctx);
        _adminComplaints = new AdminComplaints(ctx);
    }


    public void Run()
    {
        while (_ctx.loginStatus.CurrentUserInfo != null &&
               _ctx.loginStatus.CurrentUserInfo.Role == (int)UserRole.Admin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Admin Dashboard");
            string Prompt = $"Logged in as: {_ctx.loginStatus.CurrentUserInfo.Username} (Admin)";
            List<List<string>> Options = new List<List<string>>
            {
                new List<string> {"Map"},
                new List<string> {"Attractions"},
                new List<string> {"Menu management"},
                // new List<string> {"Manage Reservations"},
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

                    ParkMap.ShowMap();
                    break;
                case 1:
                    adminAttraction.Run();
                    break;

                case 2:
                    adminFoodmenu.Run();
                    break;

                // case 4:
                    //     _reservationManagement(); to be implemented
                    //     break;

                case 3:
                    _ctx.userAuth.Logout();
                    UiHelpers.Pause();
                    return;
                case 4:
                    _adminComplaints.Run();
                    UiHelpers.Pause();
                    break;
                case 5:
                    Environment.Exit(0);
                    return;

                default:
                    break;
            }
        }
    }
}
