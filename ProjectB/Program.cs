// See https://aka.ms/new-console-template for more information
public class Program
{
    public static void Main(string[] args)
    {
        var loginUI = AppFactory.CreateLoginUI();
        loginUI.StartLogin();

        var ReservationUI = AppFactory.CreateReservationUI();
        ReservationUI.StartReservation();
    }
}
