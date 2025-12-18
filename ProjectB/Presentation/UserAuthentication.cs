public class UserAuthentication
{
    private readonly UserContext _ctx;

    public UserAuthentication(UserContext ctx)
    {
        _ctx = ctx;
    }

    public void Logout()
    {
        if (UiHelpers.ChoiceHelper("=== Logout Menu ===\nAre you sure you want to logout?"))
        {
            string message = _ctx.AuthenticationLogic.Logout();
            Console.WriteLine(message);
            return;
        }
        return;
    }

    public void Login()
    {
        Console.WriteLine("=== Customer Login ===");

        if (!LoggedIn())
        {
            Console.WriteLine("Invalid username or password.\nPlease try again.");
            if (!UiHelpers.ChoiceHelper("Try again? "))
            {
                Console.WriteLine("Exiting login process.");
                return;
            }
            Login();
            return;
        }
        else
        {
            Console.WriteLine("Login successful!");
            return;
        }
    }

    protected bool LoggedIn()
    {
        return _ctx.AuthenticationLogic.AccountVerify(UiHelpers.InputRead("Username: "), UiHelpers.InputRead("Password: "));
    }

}
