class MainMenu
{
    public List<List<string>> Options;
    public string Prompt;
    public int SelectedIndexHeight;
    public int SelectedIndexWidth;
    public bool FirstDateSelected;
    public MainMenu(List<List<string>> options, string prompt)
    {
        SelectedIndexHeight = 0;
        SelectedIndexWidth = 0;
        Options = options;
        Prompt = prompt;
        FirstDateSelected = false;
    }

    public void DisplayOptions()
    {
        Console.ForegroundColor = Prompt != null && Prompt.ToLower().Contains("guest page") ? ConsoleColor.DarkGreen : ConsoleColor.White;
        Console.WriteLine(Prompt);
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i< Options.Count(); i++)
        {
            for (int j = 0; j<Options[i].Count(); j++)
            {
                Formatter(i, j);
            }
            Console.WriteLine("");
        }
    }

    private void Formatter(int x, int y)
    {
        string prefix = "";
        string suffix = "";
        if (x == SelectedIndexHeight && y == SelectedIndexWidth)
        {
            prefix = "{";
            suffix = "}";
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
        }
        else
        {
            prefix = " ";
            suffix = " ";
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.Write($"{prefix}{Options[x][y]}{suffix}");
    }

    public int[] Run()
    {
        ConsoleKey keyPressed;

        do
        {
            Console.Clear();
            DisplayOptions();

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            keyPressed = keyInfo.Key;

            if (keyPressed == ConsoleKey.UpArrow)
            {
                SelectedIndexHeight--;
                if (SelectedIndexHeight == -1)
                {
                    SelectedIndexHeight = Options.Count()-1;
                }
            }
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                SelectedIndexHeight++;
                if (SelectedIndexHeight == Options.Count())
                {
                    SelectedIndexHeight = 0;
                }
            }
            if (keyPressed == ConsoleKey.LeftArrow)
            {
                SelectedIndexWidth--;
                if (SelectedIndexWidth == -1)
                {
                    SelectedIndexWidth = Options[SelectedIndexHeight].Count() - 1;
                }
            }
            else if (keyPressed == ConsoleKey.RightArrow)
            {
                SelectedIndexWidth++;
                if (SelectedIndexWidth == Options[SelectedIndexHeight].Count())
                {
                    SelectedIndexWidth = 0;
                }
            }
        } while (keyPressed != ConsoleKey.Enter);
        FirstDateSelected = true;

        return new int[]{SelectedIndexHeight, SelectedIndexWidth};
    }

   public List<int> Run(int i)
{
    ConsoleKey keyPressed;

    do
    {
        Console.Clear();
        DisplayOptions();

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        keyPressed = keyInfo.Key;


        if (keyPressed == ConsoleKey.UpArrow)
        {
            SelectedIndexHeight--;
            if (SelectedIndexHeight == -1)
                SelectedIndexHeight = Options.Count() - 1;
        }
        else if (keyPressed == ConsoleKey.DownArrow)
        {
            SelectedIndexHeight++;
            if (SelectedIndexHeight == Options.Count())
                SelectedIndexHeight = 0;
        }

        else if (keyPressed == ConsoleKey.RightArrow)
        {
            string row = Options[SelectedIndexHeight][0];
            int colonIndex = row.LastIndexOf(':');
            int value = int.Parse(row.Substring(colonIndex + 1).Trim());
            value++;
            Options[SelectedIndexHeight][0] = row.Substring(0, colonIndex + 1) + " " + value;
        }

        else if (keyPressed == ConsoleKey.LeftArrow)
        {
            string row = Options[SelectedIndexHeight][0];
            int colonIndex = row.LastIndexOf(':');
            int value = int.Parse(row.Substring(colonIndex + 1).Trim());
            if (value > 0)
                value--;
            Options[SelectedIndexHeight][0] = row.Substring(0, colonIndex + 1) + " " + value;
        }

    } while (keyPressed != ConsoleKey.Enter);

    FirstDateSelected = true;

    List<int> counts = new List<int>();
    foreach (var row in Options)
    {
        int colonIndex = row[0].LastIndexOf(':');
        int value = int.Parse(row[0].Substring(colonIndex + 1).Trim());
        counts.Add(value);
    }

    return counts;
    }

}