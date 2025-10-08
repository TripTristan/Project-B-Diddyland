public class AccountRepository
{
    public User? GetUserByName(string username) // ? return can be null
    {
        return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}