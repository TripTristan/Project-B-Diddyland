using System.Text;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

public class MenuForm
{
    private readonly MenuLogic _menuLogic;

    public MenuForm(MenuLogic menuLogic)
    {
        _menuLogic = menuLogic;
    }

    public string FormatMenu(IEnumerable<MenuModel> items)
    {
        var list = items.ToList();
        if (list.Count == 0)
            return "Menu is empty.";

        var sb = new StringBuilder();
        sb.AppendLine("===== THEME PARK MENU =====");

        foreach (var m in list)
        {
            var nameParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(m.Food))
                nameParts.Add(m.Food!);
            if (!string.IsNullOrWhiteSpace(m.Drink))
                nameParts.Add(m.Drink!);

            var label = nameParts.Any() ? string.Join(" / ", nameParts) : "(Unnamed)";
            sb.AppendLine($"#{m.ID,-3} {label,-30} €{m.Price:0.00}");
        }

        sb.AppendLine("============================");
        return sb.ToString();
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(FormatMenu(_menuLogic.GetAll()));
            Console.WriteLine();
            Console.WriteLine("Choose an action:");
            Console.WriteLine("[1] Add FOOD");
            Console.WriteLine("[2] Add DRINK");
            Console.WriteLine("[3] Remove item");
            Console.WriteLine("[0] Exit");
            Console.Write("Your choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddFoodUI();
                    break;
                case "2":
                    AddDrinkUI();
                    break;
                case "3":
                    RemoveItemUI();
                    break;
                case "0":
                    return;
                default:
                    Pause("Unknown option. Press any key...");
                    break;
            }
        }
    }

    private void AddFoodUI()
    {
        Console.Clear();
        Console.WriteLine("=== Add FOOD ===");
        Console.WriteLine(FormatMenu(_menuLogic.GetAll()));
        Console.WriteLine();

        var name = PromptNonEmpty("Food name: ");
        var price = PromptPrice("Price (€): ");

        var result = _menuLogic.AddFood(name, price);
        Pause(result + " Press any key...");
    }

    private void AddDrinkUI()
    {
        Console.Clear();
        Console.WriteLine("=== Add DRINK ===");
        Console.WriteLine(FormatMenu(_menuLogic.GetAll()));
        Console.WriteLine();

        var name = PromptNonEmpty("Drink name: ");
        var price = PromptPrice("Price (€): ");

        var result = _menuLogic.AddDrink(name, price);
        Pause(result + " Press any key...");
    }

    private void RemoveItemUI()
    {
        Console.Clear();
        Console.WriteLine("=== Remove item ===");
        Console.WriteLine(FormatMenu(_menuLogic.GetAll()));
        Console.WriteLine();

        var id = PromptInt("Menu ID to remove: ");
        var result = _menuLogic.RemoveItem(id);

        Pause(result + " Press any key...");
    }

    private string PromptNonEmpty(string label)
    {
        while (true)
        {
            Console.Write(label);
            var input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Value cannot be empty.");
        }
    }

    private double PromptPrice(string label)
    {
        var styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        var cultures = new[] 
        { 
            CultureInfo.CurrentCulture, 
            CultureInfo.GetCultureInfo("nl-NL"), 
            CultureInfo.InvariantCulture 
        };

        while (true)
        {
            Console.Write(label);
            var input = (Console.ReadLine() ?? "").Trim();

            foreach (var c in cultures)
            {
                if (double.TryParse(input, styles, c, out var value))
                {
                    if (value < 0)
                    {
                        Console.WriteLine("Price cannot be negative.");
                        break;
                    }
                    return value;
                }
            }

            Console.WriteLine("Please enter a valid number (e.g., 2.50 or 2,50).");
        }
    }

    private int PromptInt(string label)
    {
        while (true)
        {
            Console.Write(label);
            var input = (Console.ReadLine() ?? "").Trim();
            if (int.TryParse(input, out var value))
                return value;

            Console.WriteLine("Please enter a valid integer.");
        }
    }

    private void Pause(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
