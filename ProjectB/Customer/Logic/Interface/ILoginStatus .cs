public interface ILoginStatus
{
    UserModel? CurrentUser { get; }
    void SetUser(UserModel user);
    void Clear();
}