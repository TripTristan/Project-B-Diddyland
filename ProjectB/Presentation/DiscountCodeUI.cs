using System;

public class DiscountCodeUI
{
    private readonly DiscountCodeLogic _logic;

    public DiscountCodeUI(DiscountCodeLogic logic)
    {
        _logic = logic;
    }

    public void ShowCreate()
    {
        Console.Clear();
        Console.WriteLine("=== Create Discount Code ===\n");

        Console.Write("Enter discount code name (e.g., WINTER20): ");
        string code = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter discount percentage (1–90): ");
        string? p = Console.ReadLine();
        int percentage = int.TryParse(p, out var result) ? result : 0;

        try
        {
            _logic.Create(code, percentage);
            Console.WriteLine("\n✔ Discount code created successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }

        UiHelpers.Pause();
    }
}
