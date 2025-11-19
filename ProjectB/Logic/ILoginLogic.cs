public interface ILoginLogic
{
    User? Authenticate(string account, string password);
}