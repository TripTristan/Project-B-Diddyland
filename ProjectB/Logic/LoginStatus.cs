public class LoginStatus
{
    public UserModel Guest { get; }
    public UserModel? CurrentUserInfo { get; private set; }

    public LoginStatus()
    {
        Guest = new UserModel
        {
            Id = 0,
            Name = "Guest",
            Email = "guest@local",
            Role = 0,
            Height = 0,
            Phone = "",
            DateOfBirth = ""
        };

        CurrentUserInfo = null; 
    }

    public void Login(UserModel accountInfo)
    {
        CurrentUserInfo = accountInfo;
    }

    public void Logout()
    {
        CurrentUserInfo = null;
    }
}
