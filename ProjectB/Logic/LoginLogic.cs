public static class LoginLogic
{
    public static bool AccountVerify(string username, string password)
    {
        UserModel account = UserAccess.GetByUsername(username.ToLower()); 
        if (account == null) return false;
        if (account.Password != password) return false;

        LoginStatus.Login(account);
        return true;
    }
}
