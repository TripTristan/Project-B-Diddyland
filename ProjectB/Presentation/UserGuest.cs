public class UserGuest
{
    private readonly UserReservation _reservation;
    private readonly FastPass _fastPass;
    private readonly UserHelp _userHelp;
    private readonly BookingHistory _bookingHistory;
    private readonly Profile _profile;
    private readonly Dependencies _ctx;

    public UserGuest(Dependencies ctx)
    {
        _reservation = new UserReservation(ctx);
        _fastPass = new FastPass(ctx);
        _userHelp = new UserHelp(ctx);
        _bookingHistory = new BookingHistory(ctx.bookingHistoryLogic);
        _profile = new Profile(ctx);
        _ctx = ctx;
    }

    public void Run()
    {
        while (_ctx.loginStatus.CurrentUserInfo != null &&
               _ctx.loginStatus.CurrentUserInfo.Role == 0)
        {
            string Prompt = $"Diddyland – Guest Page\nLogged in as: {_ctx.loginStatus.CurrentUserInfo.Username} (Guest)\nWhat would you like to do today?";
            List<List<string>> Options = new List<List<string>>
            {
                new List<string> {"Make a Reservation"},
                new List<string> {"Buy a fastpass for an Attraction"},
                new List<string> {"Submit a complaint"},
                new List<string> {"View the maps of our themeparks"},
                new List<string> {"View your order history"},
                new List<string> {"Manage your profile"},
                new List<string> {"Logout"}
            };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();

            switch (selectedIndex[0])
            {
                case 0:
                    _ctx.reservation.Book();
                    break;

                case 1:
                    _ctx.fastPass.Run(_ctx.loginStatus.CurrentUserInfo);
                    break;

                case 2:
                    _ctx.userHelp.Run();
                    break;

                case 3:
                    ParkMap.ShowMap();
                    UiHelpers.Pause();
                    break;

                case 4:
                    _ctx.bookingHistory.Display(_ctx.loginStatus.CurrentUserInfo.Username);
                    break;

                case 5:
                    _ctx.profile.Run();
                    break;

                case 6:
                    _ctx.userAuth.Logout();
                    return;

                default:
                    UiHelpers.Warn("Unknown option.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }
}
