using System;

public class UiHelpers
{
    public static void WriteHeader(string text, ConsoleColor color = ConsoleColor.Cyan)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine();
    }

    public static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static void Good(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Pause()
    {
        Console.Write("\nPress Enter to continue...");
        Console.ReadLine();
    }

    public static bool ChoiceHelper(string message)
    {
        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Yes"},
            new List<string> {"No"}, 
        };

        MainMenu Menu = new MainMenu(Options, message);
        Pause();
        int[] selectedIndex = Menu.Run();
        
        Console.ResetColor();

        switch (selectedIndex[0])
        {
            case 0:
                return true;
            case 1:
                return false;
            default:
                return false;
        }
    }
}
