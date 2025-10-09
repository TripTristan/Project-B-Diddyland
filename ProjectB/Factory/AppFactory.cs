public static class AppFactory
{
    // userLoginUI >>> LoginLogics >>> userAccess
    public static UserLoginUI CreateLoginUI()
    {
        var repo = new UsersAccess();
        var logic = new Loginlogic(repo);
        var ui = new UserLoginUI(logic);
        return ui;
    }

}