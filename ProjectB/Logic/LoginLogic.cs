public class LoginLogic : ILoginLogic
{
    private readonly IUserRepository _userRepo;
    private readonly ISessionService _sessionService;

    public LoginLogic(IUserRepository userRepo, ISessionService sessionService)
    {
        _userRepo = userRepo;
        _sessionService = sessionService;
    }

    public User? Authenticate(string account, string password)
    {
        var user = _userRepo.GetByAccount(account);
        if (user == null || user.Password != password) return null;
        
        _sessionService.SetCurrentUser(user);
        return user;
    }
}