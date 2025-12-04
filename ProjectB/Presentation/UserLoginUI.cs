public static class UserLoginUI
{
    public static void StartLogin()
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
                Console.WriteLine("Please try again.");

                if (!LoginAgain())
                {
                    Console.WriteLine("Exiting login process.");
                    break;
                }
            }
        }
    }



    private static bool LoginAgain()
    {
        return UiHelpers.ChoiceHelper("Try again? ");
    }


    private static bool UserLogin()
    {
        string username = Input_Read("Username: ");
        string password = Input_Read("Password: ");

        bool successOrFailure = LoginLogic.AccountVerify(username, password); 
        return successOrFailure;
    }

    
    private static string Input_Read(string text)
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