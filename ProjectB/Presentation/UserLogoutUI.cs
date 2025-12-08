public class UserLogoutUI
{
    private readonly LogoutLogic _logic;

    public UserLogoutUI(LogoutLogic logic)
    {
        _logic = logic;
    }

    public void Start()
    {
        ShowLogoutMenu();
    }

    private void ShowLogoutMenu()
    {
        
        if (UiHelpers.ChoiceHelper("=== Logout Menu ===\nAre you sure you want to logout?"))
        {
            UserLogout();
            return;
        }
        return;
    }

    private void UserLogout()
    {
        string message = _logic.Logout();
        Console.WriteLine(message);
    }
}
