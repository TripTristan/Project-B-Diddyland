public  class CartLine
{
    public  MenuModel Item { get; }
    public  int Quantity { get; set; }
    public  double LineTotal => Item.Price * Quantity;

    public  CartLine(MenuModel item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public  static string BuildLabel(MenuModel m)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(m.Food)) parts.Add(m.Food!);
        if (!string.IsNullOrWhiteSpace(m.Drink)) parts.Add(m.Drink!);
        return parts.Count > 0 ? string.Join(" / ", parts) : "(Unnamed)";
    }
}

public record OrderLineSnapshot(int ID, string Food, string Drink, double Price, int Quantity)
{
    public  string Label => CartLine.BuildLabel(new MenuModel(ID, Food, Drink, Price));
    public  double Subtotal => Price * Quantity;
}

public record OrderSummary(DateTime Timestamp, DateTime PickupTime, List<OrderLineSnapshot> Lines, double Total);
