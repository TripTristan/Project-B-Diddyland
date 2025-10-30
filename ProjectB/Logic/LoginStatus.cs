public static class LoginStatus
{
    public static UserModel? CurrentUserInfo { get; private set; } // wie is ingelogd // current user

    public static void Login(UserModel accountInfo) => CurrentUserInfo = accountInfo; 

    public static void Logout() => CurrentUserInfo = null;
}