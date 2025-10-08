public static class LoginStatus
{
    public static Account? CurrentAccount { get; private set; }

    public static void Login(Account account) => CurrentAccount= account; 

    public static void Logout() => CurrentAccount= null;
}
