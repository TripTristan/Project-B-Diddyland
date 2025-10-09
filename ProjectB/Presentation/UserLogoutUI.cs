public class UserLogoutUI
{

    private void ShowLogoutMenu()
    {
        Console.WriteLine("=== Logout Menu ===");
        Console.WriteLine("Are you sure you want to logout?\n(1) Yes \n(2) No");

        while (true)
        {
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UserLogout();
                    return;

                case "2":
                    Console.WriteLine("Logout cancelled.");
                    // go back to main menu???????
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void UserLogout()
    {
        string message = _logic.Logout();
        Console.WriteLine(message);
    }

}