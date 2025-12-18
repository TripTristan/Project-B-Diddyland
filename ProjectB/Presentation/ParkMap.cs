using System;
using System.Collections.Generic;

public class ParkMap
{
    private const int W = 102;
    private const int H = 42;

    private readonly char[,] _glyph = new char[H, W];
    private readonly int[,] _color = new int[H, W];
    private const string RESET = "\u001b[0m";

    public enum Zone { Adventure = 1, Coastal = 2, Jungle = 3, Retro = 4 }

    public void ShowMap()
    {
        List<List<string>> locationOptions = new List<List<string>>
        {
            new List<string> { "Diddyland Rotterdam" },
            new List<string> { "Diddyland Amsterdam" },
            new List<string> { "Go Back" }
        };

        MainMenu locationMenu = new MainMenu(locationOptions, "Select park location:");
        int[] result = locationMenu.Run();
        if (result[0] == 2) return;

        string location = result[0] == 1 ? "amsterdam" : "rotterdam";
        ShowInteractive(location);
    }

    public void ShowInteractive(string location)
    {
        List<List<string>> options = new List<List<string>>
        {
            new List<string> { "Adventure Zone" },
            new List<string> { "Coastal Zone" },
            new List<string> { "Jungle Zone" },
            new List<string> { "Retro Zone" },
            new List<string> { "All zones" },
            new List<string> { "Back" }
        };

        MainMenu menu = new MainMenu(options, "Which zone would you like to view?");
        int[] sel = menu.Run();
        if (sel[0] == 5) return;

        Zone? filter = sel[0] switch
        {
            0 => Zone.Adventure,
            1 => Zone.Coastal,
            2 => Zone.Jungle,
            3 => Zone.Retro,
            _ => null
        };

        Console.Clear();
        Show(location, filter);
        UiHelpers.Pause();
    }

    private void Show(string location, Zone? filter)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DrawBase();
        PlaceAll(location, filter);
        DrawLegend(location);
        Render();
        Console.WriteLine(RESET);
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

        int midX = W / 2, midY = 18;
        DrawVLine(midX, 2, 34);
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
        else
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

    private void DrawLegend(string location)
    {
        int y = H - 6;
        WriteText(3, y, $"Legend – Location: {location}", 96);
        WriteText(3, y + 1, "# - | +  Walkways", 90);
        WriteText(3, y + 2, "Red    Adventure Zone", 31);
        WriteText(3, y + 3, "Blue   Coastal Zone", 34);
        WriteText(3, y + 4, "Green  Jungle Zone", 32);
        WriteText(3, y + 5, "Yellow Retro Zone", 33);
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
}
