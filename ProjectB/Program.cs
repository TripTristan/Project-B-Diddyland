// Program.cs
using System;
using System.Globalization;
using System.Text;

class Program
{
    static void Main()
    {
        var attractiesRepo = new AttractiesAccess();
        var menusRepo = new MenusAccess();

        // Logic layers
        var attractieLogic = new AttractieLogic(attractiesRepo);
        var menuLogic = new MenuLogic(menusRepo);
        var orderLogic = new OrderLogic(menuLogic);

        // Main loop
        while (true)
        {
            Console.Clear();
            WriteHeader("Diddyland – Main Menu");

            if (LoginStatus.CurrentUserInfo != null)
                Console.WriteLine($"Logged in as: {LoginStatus.CurrentUserInfo.Username}");

            Console.WriteLine("1) Attractions");
            Console.WriteLine("2) Menu management");
            Console.WriteLine("3) Orders");
            Console.WriteLine("4) Register");
            Console.WriteLine("5) Reservation");
            Console.WriteLine("6) Login");
            Console.WriteLine("7) Logout");
            Console.WriteLine("8) Map");
            Console.WriteLine("0) Quit");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        AttractieMenu.Start(attractieLogic);
                        break;

                    case "2":
                        MenuForm.Run(menuLogic);
                        break;

                    case "3":
                        OrderForm.Run(orderLogic);
                        break;

                    case "4":
                        UserRegister.Register();
                        break;

                    case "5":
                        AppFactory.CreateReservationUI().StartReservation();
                        Pause();
                        break;

                    case "6":
                        if (LoginStatus.CurrentUserInfo != null)
                        {
                            Warn("You are already logged in.");
                        }
                        else
                        {
                            AppFactory.CreateLoginUI().StartLogin();
                        }
                        Pause();
                        break;

                    case "7":
                        if (LoginStatus.CurrentUserInfo == null)
                        {
                            Warn("No user is currently logged in.");
                        }
                        else
                        {
                            new UserLogoutUI().Start();
                        }
                        Pause();
                        break;

                    case "8":
                        ParkMap.ShowInteractive();
                        break;

                    case "0":
                        return;

                    default:
                        Warn("Unknown option.");
                        Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                Pause();
            }
        }

        static void WriteHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
            Console.WriteLine(new string('=', text.Length));
            Console.WriteLine();
        }

        static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
