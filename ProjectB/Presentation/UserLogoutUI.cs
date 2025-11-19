public class UserLogoutUI
{
    private readonly ILogoutStatus _Logoutlogic;

    public UserLogoutUI(ILogoutStatus logic)
    {
        _Logoutlogic = logic;
    }

    public void Start()
    {
        ShowLogoutMenu();
    }

    private void ShowLogoutMenu()
    {
        Console.WriteLine("\n=== Logout Menu ===");
        Console.WriteLine("Are you sure you want to logout?\n(1) Yes \n(2) No");

        while (true)
        {
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    _Logoutlogic.Clear();
                    Console.WriteLine("You have been successfully logged out.");
                    return;
                case "2":
                    Console.WriteLine("Logout cancelled.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}