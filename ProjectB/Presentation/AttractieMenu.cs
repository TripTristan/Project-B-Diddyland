public class AttractieMenu
{
    public static void Start()
    {
        RunMenu();
    }
    
    private static void RunMenu()
    {
        while (true)
        {
            Console.Clear();
            Header("Diddyland – Attractions");
            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"List all"},
                new List<string> {"View by ID"}, 
                new List<string> {"Add"}, 
                new List<string> {"Edit"},
                new List<string> {"Delete"}, 
                new List<string> {"Quit"} 
            };

            MainMenu Menu = new MainMenu(Options, "");
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();

            try
            {
                switch (choice)
                {
                    case 0: ListAll(); break;
                    case 1: ViewById(); break;
                    case 2: Add(); break;
                    case 3: Edit(); break;
                    case 4: Delete(); break;
                    case 5: return;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            Pause();
        }
    }

    private static void ListAll()
    {
        Header("All attractions");

        var items = AttractieLogic.GetAll().ToList();
        if (!items.Any())
        {
            Info("No attractions yet.");
            return;
        }

        Console.WriteLine($"{Pad("ID",5)}  {Pad("Name",25)}  {Pad("Type",15)}  {Pad("Min(cm)",8)}  {Pad("MaxCapacity",8)}");
        Console.WriteLine(new string('-', 70));
        foreach (var m in items)
        {
            Console.WriteLine($"{Pad(m.ID.ToString(),5)}  {Pad(m.Name,25)}  {Pad(m.Type,15)}  {Pad(m.MinHeightInCM.ToString(),8)}  {Pad(m.Capacity.ToString(),8)}");
        }
    }

    private static void ViewById()
    {
        Header("View attraction");
        var id = ReadInt("Enter ID", min: 1);
        var m = AttractieLogic.Get(id);
        if (m == null) { Warn($"Attraction with ID {id} not found."); return; }
        PrintOne(m);
    }

    private static void Add()
    {
        Header("Add attraction");
        var m = new AttractieModel
        {
            Name = ReadRequired("Name"),
            Type = ReadRequired("Type"),
            MinHeightInCM = ReadInt("Min height (cm)", min: 0, max: 300),
            Capacity = ReadInt("MaxCapacity", min: 1, max: 5000),
        };

        AttractieLogic.Add(m);
        Success("Added.");
    }

    private static void Edit()
    {
        Header("Edit attraction");
        var id = ReadInt("Enter ID to edit", min: 1);
        var existing = AttractieLogic.Get(id);
        if (existing == null) { Warn($"Attraction with ID {id} not found."); return; }

        Console.WriteLine("Press Enter to keep the current value shown in [brackets].");

        var name = ReadOptional($"Name [{existing.Name}]");
        var type = ReadOptional($"Type [{existing.Type}]");
        var minH = ReadOptionalInt($"Min height (cm) [{existing.MinHeightInCM}]", 0, 300);
        var cap  = ReadOptionalInt($"MaxCapacity [{existing.Capacity}]", 1, 5000);

        existing.Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name.Trim();
        existing.Type = string.IsNullOrWhiteSpace(type) ? existing.Type : type.Trim();
        existing.MinHeightInCM = minH ?? existing.MinHeightInCM;
        existing.Capacity = cap ?? existing.Capacity;

        AttractieLogic.Update(existing);
        Success("Updated.");
    }

    private static void Delete()
    {
        Header("Delete attraction");
        var id = ReadInt("Enter ID to delete", min: 1);
        var m = AttractieLogic.Get(id);
        if (m == null) { Warn($"Attraction with ID {id} not found."); return; }

        PrintOne(m);
        
        if (UiHelpers.ChoiceHelper("Are you sure you want to delete this? (y/N): "))
        {
            AttractieLogic.Delete(id);
            Success("Deleted.");
        }
        else
        {
            Info("Cancelled.");
        }
    }

    private static void PrintOne(AttractieModel m)
    {
        Console.WriteLine();
        Console.WriteLine($"ID:          {m.ID}");
        Console.WriteLine($"Name:        {m.Name}");
        Console.WriteLine($"Type:        {m.Type}");
        Console.WriteLine($"Min height:  {m.MinHeightInCM} cm");
        Console.WriteLine($"MaxCapacity:    {m.Capacity}");
        Console.WriteLine();
    }

    private static string Pad(string s, int w) => (s ?? "").Length > w ? s[..w] : s.PadRight(w);

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Warn($"{label} is required.");
        }
    }

    private static int ReadInt(string label, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v) && (!min.HasValue || v >= min) && (!max.HasValue || v <= max))
                return v;

            var range = (min, max) switch
            {
                (int a, int b) => $" between {a} and {b}",
                (int a, null)  => $" ≥ {a}",
                (null, int b)  => $" ≤ {b}",
                _              => ""
            };
            Warn($"Please enter a valid integer{range}.");
        }
    }

    private static string? ReadOptional(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine();
    }

    private static int? ReadOptionalInt(string label, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s, out var v) && (!min.HasValue || v >= min) && (!max.HasValue || v <= max))
                return v;

            var range = (min, max) switch
            {
                (int a, int b) => $" between {a} and {b}",
                (int a, null)  => $" ≥ {a}",
                (null, int b)  => $" ≤ {b}",
                _              => ""
            };
            Warn($"Please enter a valid integer{range}, or press Enter to keep.");
        }
    }

    private static void Header(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(text);
        Console.ResetColor();
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine();
    }

    private static void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Info(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}