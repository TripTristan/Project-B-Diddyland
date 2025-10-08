// See https://aka.ms/new-console-template for more information
public class Program
{
    public static void Main(string[] args)
    {
        // factory method // call userLoginUI // userLogin function
        var loginUI = AppFactory.CreateLoginUI();
        loginUI.StartLogin();
        //#########################################################


    }
}
