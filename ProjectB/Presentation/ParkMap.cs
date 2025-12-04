using System;

public static class ParkMap
{
    private const int W = 102;
    private const int H = 36;
    private static readonly char[,] glyph = new char[H, W];
    private static readonly int[,] color = new int[H, W];
    private static readonly string RESET = "\u001b[0m";

    public enum Zone { Adventure = 1, Coastal = 2, Jungle = 3, Retro = 4 }

    public static void ShowInteractive()
    {
        Zone? filter = new();
        Console.Clear();
        string Prompt = "=== Diddyland Park Map ===\n\nWhich zone would you like to view?";
        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Adventure Zone"},
            new List<string> {"Coastal Zone"}, 
            new List<string> {"Jungle Zone"}, 
            new List<string> {"Retro Zone"},
            new List<string> {"All zones"}
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();

        switch (selectedIndex[0])
        {
            case 0:
                filter = Zone.Adventure;
                break;
            case 1:
                filter = Zone.Coastal;
                break;
            case 2:
                filter = Zone.Jungle;
                break;
            case 3:
                filter = Zone.Retro;
                break;
            default:
                break;
        }


        Console.Clear();
        Show(filter);
    }

    public static void Show(Zone? filter)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DrawBase();
        PlaceAll(filter);

        Render();
        Console.WriteLine(RESET);
        PrintLegend(filter);
        UiHelpers.Pause();
    }

    // --- DRAWING METHODS ---
    private static void Clear(char ch = ' ')
    {
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            { glyph[y, x] = ch; color[y, x] = 0; }
    }

    private static void P(int x, int y, char ch)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return;
        glyph[y, x] = ch;
    }

    private static void C(int x, int y, int c)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return;
        color[y, x] = c;
    }

    private static void DrawBox(int left, int top, int width, int height, string label, int col)
    {
        int right = Math.Min(W - 1, left + width - 1);
        int bottom = Math.Min(H - 1, top + height - 1);

        for (int x = left; x <= right; x++)
        {
            P(x, top, '-');     C(x, top, col);
            P(x, bottom, '-');  C(x, bottom, col);
        }
        for (int y = top; y <= bottom; y++)
        {
            P(left, y, '|');    C(left, y, col);
            P(right, y, '|');   C(right, y, col);
        }

        P(left, top, '+');      C(left, top, col);
        P(right, top, '+');     C(right, top, col);
        P(left, bottom, '+');   C(left, bottom, col);
        P(right, bottom, '+');  C(right, bottom, col);

        // Centered label
        string text = label.Length <= width - 2 ? label : label.Substring(0, width - 2);
        int tx = left + 1 + Math.Max(0, ((width - 2) - text.Length) / 2);
        int ty = top + height / 2;
        for (int i = 0; i < text.Length && tx + i < right; i++)
        { P(tx + i, ty, text[i]); C(tx + i, ty, col); }
    }

    private static void DrawHLine(int x1, int x2, int y, int col = 90)
    {
        for (int x = Math.Max(0, x1); x <= Math.Min(W - 1, x2); x++) { P(x, y, '-'); C(x, y, col); }
    }

    private static void DrawVLine(int x, int y1, int y2, int col = 90)
    {
        for (int y = Math.Max(0, y1); y <= Math.Min(H - 1, y2); y++) { P(x, y, '|'); C(x, y, col); }
    }

    private static void DrawBase()
    {
        Clear(' ');
        for (int x = 0; x < W; x++) { P(x, 0, '#'); C(x, 0, 90); P(x, H - 1, '#'); C(x, H - 1, 90); }
        for (int y = 0; y < H; y++) { P(0, y, '#'); C(0, y, 90); P(W - 1, y, '#'); C(W - 1, y, 90); }

        WriteCentered(1, "DIDDYLAND PARK MAP", 96);

        int midX = W / 2, midY = H / 2;
        DrawVLine(midX, 1, H - 2);
        DrawHLine(1, W - 2, midY);
        P(midX, midY, '+'); C(midX, midY, 90);
    }

    private static void PlaceAll(Zone? filter)
    {
        int red = 31, blue = 34, green = 32, yellow = 33;
        bool Show(Zone z) => filter == null || filter == z;

        if (Show(Zone.Adventure))
        {
            DrawBox(4, 4, 20, 6, "Thunderbolt", red);
            DrawBox(27, 4, 20, 6, "Serpent", red);
            DrawBox(4, 12, 20, 6, "Drop Tower", red);
            DrawBox(27, 12, 20, 6, "Pizza Place", red);
            WriteText(6, 2, "Adventure Zone", red);
        }

        if (Show(Zone.Coastal))
        {
            DrawBox(55, 4, 20, 6, "Cyclone", blue);
            DrawBox(78, 4, 20, 6, "Wildcat", blue);
            DrawBox(55, 12, 20, 6, "Ferris Wheel", blue);
            DrawBox(78, 12, 20, 6, "Burger Shack", blue);
            WriteText(72, 2, "Coastal Zone", blue);
        }

        if (Show(Zone.Jungle))
        {
            DrawBox(4, 20, 20, 6, "Vortex", green);
            DrawBox(27, 20, 20, 6, "Dragon", green);
            DrawBox(4, 28, 20, 6, "Carousel", green);
            DrawBox(27, 28, 20, 6, "Toilets West", green);
            WriteText(6, 18, "Jungle Zone", green);
        }

        if (Show(Zone.Retro))
        {
            DrawBox(55, 20, 20, 6, "Comet", yellow);
            DrawBox(78, 20, 20, 6, "Riptide", yellow);
            DrawBox(55, 28, 20, 6, "Bumper Cars", yellow);
            DrawBox(78, 28, 20, 6, "Toilets East", yellow);
            WriteText(73, 18, "Retro Zone", yellow);
        }
    }

    private static void Render()
    {
        for (int y = 0; y < H; y++)
        {
            int current = 0;
            for (int x = 0; x < W; x++)
            {
                int c = color[y, x];
                if (c != current)
                {
                    Console.Write(c == 0 ? RESET : $"\u001b[{c}m");
                    current = c;
                }
                Console.Write(glyph[y, x]);
            }
            Console.WriteLine(RESET);
        }
    }

    private static void WriteText(int x, int y, string text, int col)
    {
        for (int i = 0; i < text.Length && x + i < W - 1; i++)
        { P(x + i, y, text[i]); C(x + i, y, col); }
    }

    private static void WriteCentered(int y, string text, int col)
    {
        int x = Math.Max(1, (W - text.Length) / 2);
        for (int i = 0; i < text.Length; i++) { P(x + i, y, text[i]); C(x + i, y, col); }
    }

    private static void PrintLegend(Zone? filter)
    {
        Console.WriteLine("Legend:");
        Console.WriteLine("\u001b[90m#,-,|,+\u001b[0m Walkways");
        Console.WriteLine("\u001b[31mRed   \u001b[0m Adventure Zone");
        Console.WriteLine("\u001b[34mBlue  \u001b[0m Coastal Zone");
        Console.WriteLine("\u001b[32mGreen \u001b[0m Jungle Zone");
        Console.WriteLine("\u001b[33mYellow\u001b[0m Retro Zone");
        if (filter is Zone z)
            Console.WriteLine($"\nFilter active: {z} zone only");
        else
            Console.WriteLine("\nShowing all zones");
    }

}
