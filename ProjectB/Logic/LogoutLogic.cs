public class LogoutLogic
{
    private readonly LoginStatus _loginStatus;

    public LogoutLogic(LoginStatus loginStatus)
    {
        _loginStatus = loginStatus;
    }

    public string Logout()
    {
        _loginStatus.Logout();
        return "You have been successfully logged out.";
    }
}
