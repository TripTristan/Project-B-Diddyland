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
            Console.WriteLine(_ctx.authenticationLogic.Logout());
            return;
        }
        return;
    }

    public void Login()
    {
        Console.WriteLine("=== Customer Login ===");

        if (LoggedIn())
        {
            Console.WriteLine("Login successful!");
            return;
        }
        if (UiHelpers.ChoiceHelper("Invalid username or password, Try again? "))
        {
            Login();
            return;
        }
        Console.WriteLine("Exiting login process.");
        return;
    }

    protected bool LoggedIn()
    {
        return _ctx.authenticationLogic.AccountVerify(UiHelpers.InputRead("Username: "), UiHelpers.InputRead("Password: "));
    }

}
