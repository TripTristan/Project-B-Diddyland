using System.Net.WebSockets;

public class Application
{
    private readonly LoginStatus _loginStatus;
    private readonly UserAuthentication _userAuth;
    private readonly UserRegister _register;
    private readonly UserGuest _userGuest;
    private readonly Admin _admin;
    private readonly AdminSuper _AdminSuper;
    // private readonly AdminSuper _AdminSuper;

    public Application(
        LoginStatus loginStatus,
        UserAuthentication userAuth,
        UserGuest userGuest,
        UserRegister register,
        Admin admin,
        AdminSuper AdminSuper)
    {
        _loginStatus = loginStatus;
        _userAuth = userAuth;
        _register = register;
        _userGuest = userGuest;
        _admin = admin;
        _AdminSuper = AdminSuper;
    }

    public void Run()
    {
        try
        {
            if (_loginStatus.CurrentUserInfo != null)
            {
                int role = _loginStatus.CurrentUserInfo.Role;

                if (role == 0)
                    _userGuest.Run();
                else if (role == 1)
                    _admin.Run();
                else if (role == 2)
                    _AdminSuper.Run();
                else
                {
                    UiHelpers.Warn("Invalid role. Logging out...");
                    _userAuth.Logout();
                }
            }
            ShowSplash();
            Run();
        }
        catch (Exception ex)
        {
            UiHelpers.Error(ex.Message);
            UiHelpers.Pause();
        }
    }

    private void ShowSplash()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        string Prompt = (@"
________  .___________  ________ _____.___.____       _____    _______  ________
\______ \ |   \______ \ \______ \\__  |   |    |     /  _  \   \      \ \______ \
 |    |  \|   ||    |  \ |    |  \/   |   |    |    /  /_\  \  /   |   \ |    |  \
 |    `   \   ||    `   \|    `   \____   |    |___/    |    \/    |    \|    `   \
/_______  /___/_______  /_______  / ______|_______ \____|__  /\____|__  /_______  /
        \/            \/        \/\/              \/       \/         \/        \/

===================================================================================
Welcome to our theme park application! Please choose an option to continue.
Navigate using the arrow keys and press Enter to select.
===================================================================================");

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Login"},
            new List<string> {"Register"},
            new List<string> {"Continue as Guest"},
            new List<string> {"Quit"},
        };
        Console.ResetColor();
        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        Console.ResetColor();

        switch (selectedIndex[0])
        {
            case 0:
                _userAuth.Login();
                break;
            case 1:
                _register.Register();
                // ShowSplash();
                break;
            case 2:
                EnsureGuestSession();
                _userGuest.Run();
                break;
            case 3:
                break;

            default:
                Console.WriteLine("");
                break;
        }
    }

    private void EnsureGuestSession()
    {
        _loginStatus.Login(new UserModel
        {
            Id = 0,
            Name = "Guest",
            Email = "guest@local",
            Password = "",
            Phone = "",
            Height = 0,
            DateOfBirth = "",
            Admin = 0
        });
    }
}
