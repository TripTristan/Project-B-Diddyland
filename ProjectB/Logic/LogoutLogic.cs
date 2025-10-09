public class LogoutLogic
{
    public string Logout()
    {
        LoginStatus.Logout();
        return "You have been successfully logged out.";
    }
}