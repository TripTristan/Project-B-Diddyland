using System;

public class ParkMap
{
    private const int W = 102;
    private const int H = 36;

    private readonly char[,] _glyph = new char[H, W];
    private readonly int[,] _color = new int[H, W];
    private const string RESET = "\u001b[0m";

    public enum Zone { Adventure = 1, Coastal = 2, Jungle = 3, Retro = 4 }

    public void ShowMap()
    {
        Console.Clear();
        Console.WriteLine("Select park location:");
        Console.WriteLine("1) Diddyland Rotterdam");
        Console.WriteLine("2) Diddyland Amsterdam");
        Console.Write("\nEnter choice: ");

        string? input = Console.ReadLine();
        string location = input switch
        {
            "2" => "amsterdam",
            _ => "rotterdam"
        };

        ShowInteractive(location);
    }

    public void ShowInteractive(string location)
    {
        Console.Clear();
        Console.WriteLine("=== Diddyland Park Map ===\n");
        Console.WriteLine("Which zone would you like to view?");
        Console.WriteLine("  1) Adventure Zone");
        Console.WriteLine("  2) Coastal Zone");
        Console.WriteLine("  3) Jungle Zone");
        Console.WriteLine("  4) Retro Zone");
        Console.WriteLine("  0) All zones\n");

        Console.Write("Enter a number or name: ");
        var input = Console.ReadLine()?.Trim()?.ToLowerInvariant() ?? "0";
        Zone? filter = ParseFilter(input);

        Console.Clear();
        Show(location, filter);

        Console.WriteLine();
        Console.Write("Press Enter to return...");
        Console.ReadLine();
    }

    private void Show(string location, Zone? filter)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DrawBase();
        PlaceAll(location, filter);
        Render();
        Console.WriteLine(RESET);
        PrintLegend(filter, location);
    }

    private void Clear(char ch = ' ')
    {
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                _glyph[y, x] = ch;
                _color[y, x] = 0;
            }
    }

    private void P(int x, int y, char ch)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return;
        _glyph[y, x] = ch;
    }

    private void C(int x, int y, int c)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return;
        _color[y, x] = c;
    }

    private void DrawBase()
    {
        Clear(' ');

        for (int x = 0; x < W; x++)
        {
            P(x, 0, '#'); C(x, 0, 90);
            P(x, H - 1, '#'); C(x, H - 1, 90);
        }

        for (int y = 0; y < H; y++)
        {
            P(0, y, '#'); C(0, y, 90);
            P(W - 1, y, '#'); C(W - 1, y, 90);
        }

        WriteCentered(1, "DIDDYLAND PARK MAP", 96);

        int midX = W / 2, midY = H / 2;
        DrawVLine(midX, 1, H - 2);
        DrawHLine(1, W - 2, midY);
        P(midX, midY, '+'); C(midX, midY, 90);
    }

    private void PlaceAll(string location, Zone? filter)
    {
        bool Show(Zone z) => filter == null || filter == z;

        int red = 31, blue = 34, green = 32, yellow = 33;
        location = location.ToLowerInvariant();

        if (location == "rotterdam")
        {
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
        else if (location == "amsterdam")
        {
            if (Show(Zone.Adventure))
            {
                DrawBox(5, 3, 18, 6, "Dragon Fly", red);
                DrawBox(25, 3, 18, 6, "Spin Tornado", red);
                DrawBox(5, 11, 18, 6, "Sky Drop", red);
                DrawBox(25, 11, 18, 6, "Snack Hut", red);
                WriteText(6, 1, "Adventure Zone", red);
            }
            if (Show(Zone.Coastal))
            {
                DrawBox(55, 3, 18, 6, "Blue Whirl", blue);
                DrawBox(75, 3, 18, 6, "Sea Dragon", blue);
                DrawBox(55, 11, 18, 6, "Mini Ferris", blue);
                DrawBox(75, 11, 18, 6, "Food Shack", blue);
                WriteText(72, 1, "Coastal Zone", blue);
            }
            if (Show(Zone.Jungle))
            {
                DrawBox(5, 20, 18, 6, "Jungle Ride", green);
                DrawBox(25, 20, 18, 6, "Swinger", green);
                DrawBox(5, 28, 18, 6, "Mini Carousel", green);
                DrawBox(25, 28, 18, 6, "Toilets West", green);
                WriteText(6, 18, "Jungle Zone", green);
            }
            if (Show(Zone.Retro))
            {
                DrawBox(55, 20, 18, 6, "Retro Coaster", yellow);
                DrawBox(75, 20, 18, 6, "Old Spinner", yellow);
                DrawBox(55, 28, 18, 6, "Bumper Cars", yellow);
                DrawBox(75, 28, 18, 6, "Toilets East", yellow);
                WriteText(72, 18, "Retro Zone", yellow);
            }
        }
    }

    private void Render()
    {
        for (int y = 0; y < H; y++)
        {
            int current = 0;
            for (int x = 0; x < W; x++)
            {
                int c = _color[y, x];
                if (c != current)
                {
                    Console.Write(c == 0 ? RESET : $"\u001b[{c}m");
                    current = c;
                }
                Console.Write(_glyph[y, x]);
            }
            Console.WriteLine(RESET);
        }
    }

    private void DrawBox(int left, int top, int width, int height, string label, int col)
    {
        int right = Math.Min(W - 1, left + width - 1);
        int bottom = Math.Min(H - 1, top + height - 1);

        for (int x = left; x <= right; x++)
        {
            P(x, top, '-'); C(x, top, col);
            P(x, bottom, '-'); C(x, bottom, col);
        }
        for (int y = top; y <= bottom; y++)
        {
            P(left, y, '|'); C(left, y, col);
            P(right, y, '|'); C(right, y, col);
        }

        P(left, top, '+'); C(left, top, col);
        P(right, top, '+'); C(right, top, col);
        P(left, bottom, '+'); C(left, bottom, col);
        P(right, bottom, '+'); C(right, bottom, col);

        WriteText(left + 2, top + height / 2, label, col);
    }

    private void DrawHLine(int x1, int x2, int y, int col = 90)
    {
        for (int x = Math.Max(0, x1); x <= Math.Min(W - 1, x2); x++)
            { P(x, y, '-'); C(x, y, col); }
    }

    private void DrawVLine(int x, int y1, int y2, int col = 90)
    {
        for (int y = Math.Max(0, y1); y <= Math.Min(H - 1, y2); y++)
            { P(x, y, '|'); C(x, y, col); }
    }

    private void WriteText(int x, int y, string text, int col)
    {
        for (int i = 0; i < text.Length && x + i < W - 1; i++)
            { P(x + i, y, text[i]); C(x + i, y, col); }
    }

    private void WriteCentered(int y, string text, int col)
    {
        int x = Math.Max(1, (W - text.Length) / 2);
        for (int i = 0; i < text.Length; i++)
            { P(x + i, y, text[i]); C(x + i, y, col); }
    }

    private void PrintLegend(Zone? filter, string location)
    {
        Console.WriteLine($"Legend – Location: {location}");
        Console.WriteLine("\u001b[90m#,-,|,+\u001b[0m Walkways");
        Console.WriteLine("\u001b[31mRed   \u001b[0m Adventure Zone");
        Console.WriteLine("\u001b[34mBlue  \u001b[0m Coastal Zone");
        Console.WriteLine("\u001b[32mGreen \u001b[0m Jungle Zone");
        Console.WriteLine("\u001b[33mYellow\u001b[0m Retro Zone");
    }

    private static Zone? ParseFilter(string s)
    {
        if (s is "0" or "all" or "*" or "") return null;
        if (int.TryParse(s, out int n) && n is >= 1 and <= 4) return (Zone)n;
        if (s.StartsWith("adv")) return Zone.Adventure;
        if (s.StartsWith("coa")) return Zone.Coastal;
        if (s.StartsWith("jun")) return Zone.Jungle;
        if (s.StartsWith("ret")) return Zone.Retro;
        return null;
    }
}
