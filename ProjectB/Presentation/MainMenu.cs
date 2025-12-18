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
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(Prompt);
        for (int i = 0; i< Options.Count(); i++)
        {
            for (int j = 0; j<Options[i].Count(); j++)
            {
                int x = i;
                int y = j;
                string prefix = "";
                string suffix = "";
                
                if (x == SelectedIndexHeight && y == SelectedIndexWidth)
                {
                    prefix = "{";
                    suffix = "}";
                    if (FirstDateSelected)
                    {
                        Console.ForegroundColor = ConsoleColor. Green;
                        
                    }
                    else
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
                Console.Write($"{prefix}{Options[i][j]}{suffix}");
            }
            Console.WriteLine("");
        }
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
            else if (keyPressed == ConsoleKey.LeftArrow)
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