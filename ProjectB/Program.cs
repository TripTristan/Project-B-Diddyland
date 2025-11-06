enum UserRole
{
    User = 0,        
    Admin = 1,
    SuperAdmin = 2
}

class Program
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
                    Console.Write("Choose an option: ");
                    var choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1": 
                            UserLoginUI.StartLogin();
                            UiHelpers.Pause();
                            break;

                        case "2": 
                            UserRegister.Register();
                            UiHelpers.Pause();
                            break;

                        case "3": 
                            EnsureGuestSession();
                            GuestMenu.Run();
                            break;

                        case "0": 
                            return;

                        default:
                            UiHelpers.Warn("Unknown option.");
                            UiHelpers.Pause();
                            break;
                    }
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

        static void ShowSplash()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
________  .___________  ________ _____.___.____       _____    _______  ________   
\______ \ |   \______ \ \______ \\__  |   |    |     /  _  \   \      \ \______ \  
 |    |  \|   ||    |  \ |    |  \/   |   |    |    /  /_\  \  /   |   \ |    |  \ 
 |    `   \   ||    `   \|    `   \____   |    |___/    |    \/    |    \|    `   \
/_______  /___/_______  /_______  / ______|_______ \____|__  /\____|__  /_______  /
        \/            \/        \/\/              \/       \/         \/        \/ 
");
            Console.ResetColor();
            Console.WriteLine("===================================================================================");
            Console.WriteLine("1) Login");
            Console.WriteLine("2) Register");
            Console.WriteLine("3) Continue as Guest");
            Console.WriteLine("0) Quit");
            Console.WriteLine();
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
}
