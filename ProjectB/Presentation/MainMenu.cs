class MainMenu
{
    List<List<string>> Options;
    string Prompt;
    int SelectedIndexHeight;
    int SelectedIndexWidth;
    bool FirstDateSelected;
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
            if (keyPressed == ConsoleKey.LeftArrow)
            {
                SelectedIndexWidth--;
                if (SelectedIndexWidth == -1)
                {
                    SelectedIndexWidth = Options.Count()+1;
                }
            }
            else if (keyPressed == ConsoleKey.RightArrow)
            {
                SelectedIndexWidth++;
                if (SelectedIndexWidth == Options.Count()+2)
                {
                    SelectedIndexWidth = 0;
                }
            } 
        } while (keyPressed != ConsoleKey.Enter);
        FirstDateSelected = true;

        return new int[]{SelectedIndexHeight, SelectedIndexWidth};
    }
}