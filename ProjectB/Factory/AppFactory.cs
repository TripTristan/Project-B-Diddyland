public static class AppFactory
{

    // factory method
    // userLoginUI >>> userLogics >>> userRepository
    public static UserLoginUI CreateLoginUI()
    {
        var repo = new AccountRepository();
        var logic = new AcounyntLogicLogic(repo);
        var ui = new UserLoginUI(logic);
        return ui;
    }
    //#########################################################


}