public class UserGuest
{
    private readonly UserReservation _reservation;
    private readonly FastPass _fastPass;
    private readonly UserHelp _userHelp;
    private readonly BookingHistory _bookingHistory;
    private readonly Profile _profile;
    private readonly UserContext _ctx;

    public UserGuest(UserContext ctx)
    {
        _reservation = new UserReservation(ctx);
        _fastPass = new FastPass(ctx);
        _userHelp = new UserHelp(ctx);
        _bookingHistory = new BookingHistory(ctx);
        _profile = new Profile(ctx);
        _ctx = ctx;
    }

    public void Run()
    {
        while (_ctx.loginStatus.CurrentUserInfo != null &&
               _ctx.loginStatus.CurrentUserInfo.Role == 0)
        {
            string Prompt = $"Diddyland – Guest Page\nLogged in as: {_ctx.loginStatus.CurrentUserInfo.Username} (Guest)";
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
                    _ctx.reservation.Book();
                    break;

                case 1:
                    _ctx.fastPass.Run(_ctx.loginStatus.CurrentUserInfo);
                    break;

                case 2:
                    _ctx.userHelp.Run();
                    UiHelpers.Pause();
                    break;

                case 3:
                    ParkMap.ShowMap();
                    UiHelpers.Pause();
                    break;

                case 4:
                    _ctx.bookingHistory.Display(_ctx.loginStatus.CurrentUserInfo.Username);
                    UiHelpers.Pause();
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
