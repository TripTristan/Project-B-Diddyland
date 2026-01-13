public class AuthenticationLogic
{
    private readonly UserAccess _userAccess;
    private readonly LoginStatus _status;

    public AuthenticationLogic(LoginStatus status, UserAccess userAccess)
    {
        _status = status;
        _userAccess = userAccess;
    }

    public string Logout()
    {
        _status.Logout();
        return "You have been successfully logged out.";
    }
    public bool AccountVerify(string username, string password)
    {
        var account = _userAccess.GetByUsername(username.ToLower());
        if (account == null) return false;
        if (account.Password != password) return false;

        _status.Login(account);
        return true;
    }
}