public static class UserLoginUI
{
    public static void StartLogin()
    {
        Console.WriteLine("=== Customer Login ===");

        while (true)
        {
            string username = Input_Read("Username: ");
            string password = Input_Read("Password: ");

            var user = UserAccess.GetByUsername(username);
            if (user != null && user.Password == password)
            {
                // Map UserModel to LoginStatus
                LoginStatus.Login(user);
                Console.WriteLine("Login successful!");
                Thread.Sleep(500);
                return;
            }

            Console.WriteLine("Invalid username or password.");
            if (!LoginAgain())
            {
                Console.WriteLine("Exiting login process.");
                return;
            }
        }
    }

    private static bool LoginAgain()
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

    private static string Input_Read(string text)
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