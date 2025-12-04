enum UserRole
{
    User = 0,        
    Admin = 1,
    SuperAdmin = 2
}

partial class Program
{
    static void Main()
    {   
        LoginStatus.Logout();
        
        while (true)
        {
            try
            {
                if (LoginStatus.CurrentUserInfo == null)
                {
                    ShowSplash();

                }
                else
                {
                    var role = LoginStatus.CurrentUserInfo.Role;
                    if (role == (int)UserRole.User)
                    {
                        GuestMenu.Run();
                    }
                    else if (role == (int)UserRole.Admin)
                    {
                        AdminMenu.Run();
                    }
                    else if (role == (int)UserRole.SuperAdmin)
                    {
                        SuperAdminMenu.Run();
                    }
                    else
                    {
                        UiHelpers.Warn("Unknown role. Logging out for safety.");
                        new UserLogoutUI().Start();
                        UiHelpers.Pause();
                    }
                }
            }
            catch (Exception ex)
            {
                UiHelpers.Error(ex.Message);
                UiHelpers.Pause();
            }
        }
    }


        static void ShowSplash()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            string Prompt = @"
________  .___________  ________ _____.___.____       _____    _______  ________   
\______ \ |   \______ \ \______ \\__  |   |    |     /  _  \   \      \ \______ \  
 |    |  \|   ||    |  \ |    |  \/   |   |    |    /  /_\  \  /   |   \ |    |  \ 
 |    `   \   ||    `   \|    `   \____   |    |___/    |    \/    |    \|    `   \
/_______  /___/_______  /_______  / ______|_______ \____|__  /\____|__  /_______  /
        \/            \/        \/\/              \/       \/         \/        \/ 

===================================================================================";

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Login"},
            new List<string> {"Register"}, 
            new List<string> {"Continue as Guest"}, 
            new List<string> {"Quit"},
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        
        Console.ResetColor();

        switch (selectedIndex[0])
        {
            case 0: 
                UserLoginUI.StartLogin();
                UiHelpers.Pause();
                break;

            case 1: 
                UserRegister.Register();
                UiHelpers.Pause();
                break;

            case 2: 
                EnsureGuestSession();
                GuestMenu.Run();
                break;

            case 3: 
                return;
            

            default:
                Console.WriteLine("");
                break;
        }
 
        }

        static void EnsureGuestSession()
        {
            LoginStatus.Login(new UserModel
            {
                Id = 0,
                Name = "Guest",
                Email = "guest@local",
                DateOfBirth = "",
                Height = 0,
                Phone = "",
                Password = "",
                Admin = 0 
            });
        }
}
