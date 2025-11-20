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

            Console.WriteLine("1) List all");
            Console.WriteLine("2) View by ID");
            Console.WriteLine("3) Add");
            Console.WriteLine("4) Edit");
            Console.WriteLine("5) Delete");
            Console.WriteLine("6) Quit");
            Console.WriteLine();

            var choice = ReadInt("Choose an option", min: 1, max: 6);
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case 1: ListAll(); break;
                    case 2: ViewById(); break;
                    case 3: Add(); break;
                    case 4: Edit(); break;
                    case 5: Delete(); break;
                    case 6: return;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

            Pause();
        }
    }

    private static string ChooseLocation()
    {
        string[] locations = { "DiddyLand - Amsterdam", "DiddyLand - Rotterdam" };
        Console.WriteLine("Select park location:");
        for (int i = 0; i < locations.Length; i++)
            Console.WriteLine($"{i + 1}) {locations[i]}");

        Console.Write("\nEnter choice number: ");
        string? input = Console.ReadLine();
        int choice = int.TryParse(input, out int n) ? n : 1;
        if (choice < 1 || choice > locations.Length) choice = 1;

        return locations[choice - 1];
    }

    private static void ListAll()
    {
        Header("All attractions");
        string location = ChooseLocation();

        var items = AttractieLogic.GetAll().Where(a => a.Location == location).ToList();
        if (!items.Any())
        {
            Info("No attractions yet for this location.");
            return;
        }

        Console.WriteLine($"{Pad("ID",5)}  {Pad("Name",25)}  {Pad("Type",15)}  {Pad("Min(cm)",8)}  {Pad("MaxCapacity",8)}  {Pad("Location",15)}");
        Console.WriteLine(new string('-', 85));
        foreach (var m in items)
        {
            Console.WriteLine($"{Pad(m.ID.ToString(),5)}  {Pad(m.Name,25)}  {Pad(m.Type,15)}  {Pad(m.MinHeightInCM.ToString(),8)}  {Pad(m.Capacity.ToString(),8)}  {Pad(m.Location,15)}");
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
        string location = ChooseLocation();

        var m = new AttractieModel
        {
            Name = ReadRequired("Name"),
            Type = ReadRequired("Type"),
            MinHeightInCM = ReadInt("Min height (cm)", min: 0, max: 300),
            Capacity = ReadInt("MaxCapacity", min: 1, max: 5000),
            Location = location
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

        string location = ReadOptional($"Location [{existing.Location}]") ?? existing.Location;

        existing.Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name.Trim();
        existing.Type = string.IsNullOrWhiteSpace(type) ? existing.Type : type.Trim();
        existing.MinHeightInCM = minH ?? existing.MinHeightInCM;
        existing.Capacity = cap ?? existing.Capacity;
        existing.Location = location;

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
        Console.Write("Are you sure you want to delete this? (y/N): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (confirm == "y" || confirm == "yes")
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
        Console.WriteLine($"MaxCapacity: {m.Capacity}");
        Console.WriteLine($"Location:    {m.Location}");
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
                (int a, null) => $" ≥ {a}",
                (null, int b) => $" ≤ {b}",
                _ => ""
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
                (int a, null) => $" ≥ {a}",
                (null, int b) => $" ≤ {b}",
                _ => ""
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
