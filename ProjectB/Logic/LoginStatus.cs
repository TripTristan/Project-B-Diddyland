public static class LoginStatus
{
    public static UserModel? CurrentAccountInfo { get; private set; }

    public static void Login(UserModel accountInfo) => CurrentAccountInfo= accountInfo; 

    public static void Logout() => CurrentAccountInfo = null;
}
