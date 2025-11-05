public static class LoginStatus
{
    public static UserModel guest = new(0, "GUEST", "", "", 170, "", "");
    public static UserModel? CurrentUserInfo = guest; // wie is ingelogd // current user

    public static void Login(UserModel accountInfo) => CurrentUserInfo = accountInfo; 

    public static void Logout() => CurrentUserInfo = guest;
}