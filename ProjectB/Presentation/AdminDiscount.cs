using System;

public class DiscountCode
{
    private AdminContext _ctx;
    public DiscountCode(AdminContext a) { _ctx = a; }


    public void CreateDiscountCode()
    {
        Console.Clear();
        UiHelpers.Warn("=== Create Discount Code ===\n");
        UiHelpers.WriteHeader("Enter discount code name (e.g., WINTER20): ");
        string code = Console.ReadLine()?.Trim() ?? "";

        UiHelpers.WriteHeader("Enter discount percentage (1–90): ");
        string? p = Console.ReadLine();
        double percentage = double.TryParse(p, out var result) ? result : 0;

        try
        {
            _ctx.discountLogic.Create(code, percentage);
            UiHelpers.Good("\n✔ Discount code created successfully!");
        }
        catch (Exception ex)
        {
            UiHelpers.Error($"\n❌ Error: {ex.Message}");
        }

        UiHelpers.Pause();
    }
}
