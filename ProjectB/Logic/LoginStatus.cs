public static class LoginStatus
{
    private static UserModel? _currentUserInfo;
    public static UserModel? CurrentUserInfo => _currentUserInfo;
    public static UserModel? CurrentUser => _currentUserInfo;
    
    public static UserModel guest = new UserModel
    {
        Id = 0,
        Name = "Guest",
        Username = "",
        Email = "guest@gmail.com",
        DateOfBirth = "",
        Height = 0,
        Phone = "",
        Password = "",
        Admin = 0,
        Role = 0
    };

    public static void Login(UserModel user)
    {
        _currentUserInfo = user;
    }

    public static void Logout()
    {
        _currentUserInfo = null;
    }
}


