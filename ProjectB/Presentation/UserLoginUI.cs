public class UserLoginUI
{
    private readonly ILoginLogic _logic;
    public UserLoginUI(ILoginLogic logic) => _logic = logic;

    public User? StartLogin()
    {
        Console.WriteLine("=== Customer Login ===");

        while (true)
        {
            string username = Input_Read("Username: ");
            string password = Input_Read("Password: ");

            var user = _logic.Authenticate(username, password);
            if (user != null)
            {
                Console.WriteLine("Login successful!");
                Thread.Sleep(500);
                return user;
            }

            Console.WriteLine("Invalid username or password.");
            if (!LoginAgain())
            {
                Console.WriteLine("Exiting login process.");
                return null;
            }
        }
    }

    private bool LoginAgain()
    {
        Console.Write("Try again? (y/n): ");
        string? choice = Console.ReadLine()?.Trim().ToLower();
        while (true)
        {
            if (choice == "y") return true;
            if (choice == "n") return false;
            Console.Write("Invalid input. Please enter 'y' or 'n': ");
            choice = Console.ReadLine()?.Trim().ToLower();
        }
    }

    private string Input_Read(string text)
    {
        string? input;
        do
        {
            Console.Write(text);
            input = Console.ReadLine()?.Trim();
        } while (string.IsNullOrEmpty(input));
        return input!;
    }
}