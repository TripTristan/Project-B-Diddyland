// See https://aka.ms/new-console-template for more information
public class Program
{
    public static void Main(string[] args)
    {
        // Login page // login UI
        //#########################################################
        // factory method // call userLoginUI // userLogin function
        var loginUI = AppFactory.CreateLoginUI();
        loginUI.StartLogin();
        //#########################################################

        // Logout page // logout UI
        //#########################################################


        


    }
}
