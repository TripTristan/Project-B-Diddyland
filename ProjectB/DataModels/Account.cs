public class Account
{
    public string Username { get; }
    public string Password { get; }

    public Account(string username, string password)
    {
        Username = username;
        Password = password;
    }
}