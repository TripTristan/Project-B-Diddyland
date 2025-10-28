public class LoginLogic
{
    private readonly UserAccess _repository;

    public LoginLogic(UserAccess repository)
    {
        _repository = repository;
    }

    public bool AccountVerify(string username, string password)
    {
        var account = _repository.GetByUsername(username); 
        if (account == null) return false;
        if (account.Password != password) return false;

        LoginStatus.Login(account);
        return true;
    }
}
