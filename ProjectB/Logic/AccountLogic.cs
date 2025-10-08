public class AccountLogic
{
    private readonly AccountRepository _repository;

    public AccountLogicLogic(AccountRepository repository)
    {
        _repository = repository;
    }

    public bool AccountVerify(string username, string password)
    {
        var account = _repository.GetUserByName(username);

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
            LoginStatus.Login(account); // set current user here
            return true;
        }
    }

    //#########################################################################################
    //UserLogoutUI>> here (Logic) >> LoginStatus
    public string Logout()
    {
        LoginStatus.Logout();
        return "You have been successfully logged out.";
    }
    //#########################################################################################

}