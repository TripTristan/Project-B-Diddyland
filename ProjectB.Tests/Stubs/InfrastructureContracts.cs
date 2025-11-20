public interface IUserRepository
{
    User? GetByAccount(string account);
}

public interface ISessionService
{
    void SetCurrentUser(User user);
    void ClearCurrentUser();
}

public interface ILogoutStatus
{
    void Clear();
}

