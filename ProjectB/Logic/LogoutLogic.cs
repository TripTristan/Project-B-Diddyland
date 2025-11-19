public class LogoutLogic : ILogoutLogic
{
    private readonly ISessionService _sessionService;

    public LogoutLogic(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public void Logout()
    {
        _sessionService.ClearCurrentUser();
    }
}