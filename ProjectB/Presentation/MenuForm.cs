public static class MenuForm
{
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

            var label = nameParts.Count > 0 ? string.Join(" / ", nameParts) : "(Unnamed)";
            sb.AppendLine($"#{m.MenuID,-3} {label,-30} €{m.Price:0.00}");
        }

        sb.AppendLine("============================");
        return sb.ToString();
    }
}