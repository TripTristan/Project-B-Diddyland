using System;
using System.Linq;

public class AdminAttraction
{
    private AdminContext _ctx;
    public AdminAttraction(AdminContext a) { _ctx = a; }

    public void Run()
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
            switch (selectedIndex[0])
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


    private void ListAll()
    {
        Header("All attractions");
        string location = ParkMap.ChooseLocation();

        var items = _ctx.attractionLogic.GetAll()
                          .Where(a => a.Location == location)
                          .ToList();

        if (!items.Any())
        {
            Info("No attractions yet for this location.");
            return;
        }

        Console.WriteLine($"{Pad("ID",5)}  {Pad("Name",25)}  {Pad("Type",15)}  {Pad("Min(cm)",8)}  {Pad("Capacity",8)}  {Pad("Location",15)}");
        Console.WriteLine(new string('-', 85));

        foreach (var m in items)
        {
            Console.WriteLine(
                $"{Pad(m.ID.ToString(),5)}  " +
                $"{Pad(m.Name,25)}  " +
                $"{Pad(m.Type,15)}  " +
                $"{Pad(m.MinHeightInCM.ToString(),8)}  " +
                $"{Pad(m.Capacity.ToString(),8)}  " +
                $"{Pad(m.Location,15)}");
        }
    }

    private void ViewById()
    {
        Header("View attraction");
        int id = ReadInt("Enter ID", min: 1);

        var m = _ctx.attractionLogic.Get(id);
        if (m == null)
        {
            Warn($"Attraction with ID {id} not found.");
            return;
        }

        PrintOne(m);
    }

    private void Add()
    {
        Header("Add attraction");

        string location = ParkMap.ChooseLocation();

        var model = new AttractieModel
        {
            Name = ReadRequired("Name"),
            Type = ReadRequired("Type"),
            MinHeightInCM = ReadInt("Min height (cm)", min: 0, max: 300),
            Capacity = ReadInt("Max capacity", min: 1, max: 5000),
            Location = location
        };

        _ctx.attractionLogic.Add(model);
        Success("Added.");
    }

    private void Edit()
    {
        Header("Edit attraction");
        int id = ReadInt("Enter ID to edit", min: 1);

        var existing = _ctx.attractionLogic.Get(id);
        if (existing == null)
        {
            Warn($"Attraction with ID {id} not found.");
            return;
        }

        Console.WriteLine("Press Enter to keep the current value (inside brackets).");

        string? name = ReadOptional($"Name [{existing.Name}]");
        string? type = ReadOptional($"Type [{existing.Type}]");
        int? minH = ReadOptionalInt($"Min height (cm) [{existing.MinHeightInCM}]", 0, 300);
        int? cap = ReadOptionalInt($"Max capacity [{existing.Capacity}]", 1, 5000);
        string? locInput = ReadOptional($"Location [{existing.Location}]");

        existing.Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name.Trim();
        existing.Type = string.IsNullOrWhiteSpace(type) ? existing.Type : type.Trim();
        existing.MinHeightInCM = minH ?? existing.MinHeightInCM;
        existing.Capacity = cap ?? existing.Capacity;
        existing.Location = string.IsNullOrWhiteSpace(locInput) ? existing.Location : locInput.Trim();

        _ctx.attractionLogic.Update(existing);
        Success("Updated.");
    }

    private void Delete()
    {
        Header("Delete attraction");
        int id = ReadInt("Enter ID to delete", min: 1);

        var existing = _ctx.attractionLogic.Get(id);
        if (existing == null)
        {
            Warn($"Attraction with ID {id} not found.");
            return;
        }

        PrintOne(existing);

        if (UiHelpers.ChoiceHelper("Are you sure you want to delete this? (y/N): "))
        {
            _ctx.attractionLogic.Delete(id);
            Success("Deleted.");
        }
        else
        {
            Info("Cancelled.");
        }
    }

    private void PrintOne(AttractieModel m)
    {
        Console.WriteLine();
        Console.WriteLine($"ID:          {m.ID}");
        Console.WriteLine($"Name:        {m.Name}");
        Console.WriteLine($"Type:        {m.Type}");
        Console.WriteLine($"Min height:  {m.MinHeightInCM} cm");
        Console.WriteLine($"Capacity:    {m.Capacity}");
        Console.WriteLine($"Location:    {m.Location}");
        Console.WriteLine();
    }

    private string Pad(string s, int w)
        => (s ?? "").Length > w ? s[..w] : s.PadRight(w);

    private string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s))
                return s.Trim();

            Warn($"{label} is required.");
        }
    }

    private int ReadInt(string label, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? s = Console.ReadLine();

            if (int.TryParse(s, out int value) &&
                (!min.HasValue || value >= min) &&
                (!max.HasValue || value <= max))
            {
                return value;
            }

            string range = min switch
            {
                int a when max is int b => $"Between {a} and {b}",
                int a                   => $"≥ {a}",
                _ when max is int b     => $"≤ {b}",
                _                       => ""
            };

            Warn($"Enter a valid number {range}.");
        }
    }

    private string? ReadOptional(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine();
    }

    private int? ReadOptionalInt(string label, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? s = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(s))
                return null;

            if (int.TryParse(s, out int v) &&
                (!min.HasValue || v >= min) &&
                (!max.HasValue || v <= max))
            {
                return v;
            }

            Warn("Invalid number.");
        }
    }

    private void Header(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(text);
        Console.ResetColor();
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine();
    }

    private void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void Info(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}
