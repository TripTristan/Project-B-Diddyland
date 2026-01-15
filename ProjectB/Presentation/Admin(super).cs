using System;

public class AdminSuper
{
    private readonly AdminFinance _adminFinance;
    private readonly ManageAdmins _manageAdmins;
    private readonly DiscountCode _adminDiscount;
    private readonly Dependencies _ctx;

    public AdminSuper(Dependencies ctx)
    {
        _adminFinance = new AdminFinance(ctx);
        _manageAdmins = new ManageAdmins(ctx);
        _adminDiscount = new DiscountCode(ctx);
        _ctx = ctx;
    }

    public void Run()
    {
        while (_ctx.loginStatus.CurrentUserInfo != null &&
               _ctx.loginStatus.CurrentUserInfo.Role == (int)UserRole.SuperAdmin)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Diddyland – Super Admin Dashboard");
            string Prompt = $"Logged in as: {_ctx.loginStatus.CurrentUserInfo.Username} (Super Admin)";
            List<List<string>> Options = new List<List<string>>
            {
                new List<string> {"Financial Dashboard"},
                new List<string> {"Attraction management"},
                new List<string> {"Menu management"},
                // new List<string> {"Manage Reservations"},
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
                    _ctx.adminFinance.Run();
                    break;
                case 1:
                    _ctx.adminAttraction.Run();
                    break;
                case 2:
                    _ctx.adminFoodmenu.Run();
                    break;
                // case 3:
                //     _reservationManagement(); to be implemented
                //     break;
                // case 3:
                //     UiHelpers.Pause();
                //     break;

                case 3:
                    ParkMap.ShowMap();
                    break;

                case 4:
                    _ctx.adminManagement.Run();
                    break;

                case 5:
                    _ctx.adminComplaints.Run();
                    break;
                case 6:
                    _ctx.discount.CreateDiscountCode();
                    break;
                case 7:
                    _ctx.userAuth.Logout();
                    return;
                case 8:
                    Environment.Exit(0);
                    return;
                default:
                    break;
            }
        }
    }
}
