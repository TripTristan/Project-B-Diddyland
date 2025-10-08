public class UserLogic
{
    private readonly AccountRepository _repository;

    public UserLogic(AccountRepository repository)
    {
        _repository = repository;
    }

    public bool AccountVerify(string username, string password)
    {
        var user = _repository.GetUserByName(username);

        if (user == null)
            return false;

        return user.Password == password;
    }
}