public class LoginLogic
{
    private readonly UserAccess _userAccess;
    private readonly LoginStatus _loginStatus;

    public LoginLogic(UserAccess userAccess, LoginStatus loginStatus)
    {
        _userAccess = userAccess;
        _loginStatus = loginStatus;
    }

    public bool AccountVerify(string username, string password)
    {
        var account = _userAccess.GetByUsername(username.ToLower());
        if (account == null) return false;
        if (account.Password != password) return false;

        _loginStatus.Login(account);
        return true;
    }
}
