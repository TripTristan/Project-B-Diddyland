// Program.cs
using System;
using System.Globalization;
using System.Text;

class Program
{
    static void Main()
    {
        var attractiesRepo = new AttractiesAccess();
        var menusRepo      = new MenusAccess();

        // Logic layers
        var attractieLogic = new AttractieLogic(attractiesRepo);
        var menuLogic      = new MenuLogic(menusRepo);
        var orderLogic     = new OrderLogic(menuLogic);

        // --- Main loop ---
        while (true)
        {
            Console.Clear();
            WriteHeader("Diddyland – Main Menu");

            Console.WriteLine("1) Attractions");
            Console.WriteLine("2) Menu management");
            Console.WriteLine("3) Orders");
            Console.WriteLine("0) Quit");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        // Opens the attractions CRUD menu you provided
                        AttractieMenu.Start(attractieLogic);
                        break;

                    case "2":
                        // Opens the menu admin UI (add/remove food/drinks)
                        MenuForm.Run(menuLogic);
                        break;

                    case "3":
                        // Opens the ordering flow with cart and checkout
                        OrderForm.Run(orderLogic);
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
    }

    // --- Small console helpers (kept local to Program.cs) ---
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
