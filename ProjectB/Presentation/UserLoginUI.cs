public class UserLoginUI
{
    private readonly UserLogic _logic;

    public UserLoginUI(UserLogic logic)
    {
        _logic = logic;
    }


    // start point of Login function
    public void StartLogin()
    {
        Console.WriteLine("=== Customer Login  ===");

        while (true)
        {
            bool success = UserLogin();

            if (success)
            {
                Console.WriteLine("Login successful!");

                break;
            }
            else
            {
                Console.WriteLine("Invalid username or password.");

                if (!LoginAgain())
                {
                    Console.WriteLine("Exiting login process.");
                    break;
                }
            }
        }
    }


    // do while loop >>> Try again? (y/n) >>> return true/false
    private bool LoginAgain()
    {
        Console.Write("Try again? (y/n): ");
        string choice = Console.ReadLine()?.Trim().ToLower();
        
        do
        {
            if (choice == "y") return true;
            if (choice == "n") return false;

            Console.Write("Invalid input. Please enter 'y' or 'n': ");
            choice = Console.ReadLine()?.Trim().ToLower();
        } while (true);
    }


    private bool UserLogin()
    {
        string username = Input_Read("Username: ");
        string password = Input_Read("Password: ");

        // Call the logic layer >>>> userLogics
        bool successOrFailure = _logic.AccountVerify(username, password); 
        return successOrFailure;
    }

    
    // do while loop >>> non-empty input
    private string Input_Read(string text)
    {
        string input;
        do
        {
            Console.Write(text);
            input = Console.ReadLine()?.Trim();
        } while (string.IsNullOrEmpty(input));
        return input;
    }
}
