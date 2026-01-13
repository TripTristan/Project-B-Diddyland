using System.Text;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

public class AdminFoodmenu
{
    private Dependencies _ctx;
    public AdminFoodmenu(Dependencies a) { _ctx = a; }


    public static string FormatMenu(IEnumerable<MenuModel> items)
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
        Console.WriteLine(FormatMenu(_ctx.foodmenuLogic.GetAll())); // only run this for non amdin users
        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Add FOOD"},
            new List<string> {"Add DRINK"},
            new List<string> {"Remove item"},
            new List<string> {"Exit"}
        };

        MainMenu Menu = new MainMenu(Options, "Choose an action:");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        switch (selectedIndex[0])
        {
            case 0:
                AddFoodUI();
                break;
            case 1:
                AddDrinkUI();
                break;
            case 2:
                RemoveItemUI();
                break;
            case 3:
                return;
            default:
                break;
        }
    }

    private void AddFoodUI()
    {
        Console.Clear();
        Console.WriteLine("=== Add FOOD ===");
        Console.WriteLine(FormatMenu(_ctx.foodmenuLogic.GetAll()));
        Console.WriteLine();

        var name = PromptNonEmpty("Food name: ");
        var price = PromptPrice("Price (€): ");

        var result = _ctx.foodmenuLogic.AddFood(name, price);
        UiHelpers.Pause(result + " Press any key...");
    }

    private void AddDrinkUI()
    {
        Console.Clear();
        Console.WriteLine("=== Add DRINK ===");
        Console.WriteLine(FormatMenu(_ctx.foodmenuLogic.GetAll()));
        Console.WriteLine();

        var name = PromptNonEmpty("Drink name: ");
        var price = PromptPrice("Price (€): ");

        var result = _ctx.foodmenuLogic.AddDrink(name, price);
        UiHelpers.Pause(result + " Press any key...");
    }

    private void RemoveItemUI()
    {
        Console.Clear();
        Console.WriteLine("=== Remove item ===");
        Console.WriteLine(FormatMenu(_ctx.foodmenuLogic.GetAll()));
        Console.WriteLine();

        var id = PromptInt("Menu ID to remove: ");
        var result = _ctx.foodmenuLogic.RemoveItem(id);

        UiHelpers.Pause(result + " Press any key...");
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

        Console.Write(label);
        var input = (Console.ReadLine() ?? "").Trim();

        foreach (var c in cultures)
        {
            if (double.TryParse(input, styles, c, out var value) && value > 0)
            {
                return value;
            }
        }

        Console.WriteLine("Please enter a valid number non negative number (e.g., 2.50 or 2,50).");
        return PromptPrice(label);
    }

    private int PromptInt(string label)
    {

        Console.Write(label);
        var input = (Console.ReadLine() ?? "").Trim();
        if (int.TryParse(input, out var value))
            return value;

        Console.WriteLine("Please enter a valid integer.");
        return PromptInt(label);

    }
}
