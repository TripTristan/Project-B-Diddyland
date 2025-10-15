public class LoginLogic
{
    private readonly UserAccess _repository;

    public AccountLogicLogic(UserAccess repository)
    {
        _repository = repository;
    }

    public bool AccountVerify(string username, string password)
    {
        var account = _repository.GetByUsername(username);

        if (account == null)
        {
            return false;
        }

        if (account.Password != password)
        {
            return false;
        }

        else
        {
            LoginStatus.Login(account); // set current user
            return true;
        }
    }

}