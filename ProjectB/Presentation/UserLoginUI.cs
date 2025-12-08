public class UserLoginUI
{
    private readonly LoginLogic _loginLogic;

    public UserLoginUI(LoginLogic loginLogic)
    {
        _loginLogic = loginLogic;
    }

    public void StartLogin()
    {
        Console.WriteLine("=== Customer Login ===");

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

    private bool LoginAgain()
    {
        return UiHelpers.ChoiceHelper("Try again? ");
    }

    private bool UserLogin()
    {
        string username = InputRead("Username: ");
        string password = InputRead("Password: ");

        return _loginLogic.AccountVerify(username, password);
    }

    private string InputRead(string text)
    {
        string input;
        do
        {
            Console.Write(text);
            input = Console.ReadLine()?.Trim();
        }
        while (string.IsNullOrEmpty(input));

        return input;
    }
}
