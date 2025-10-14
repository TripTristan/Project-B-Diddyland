public class OrderLogic
{
    private readonly MenuLogic _menuLogic;
    private readonly List<CartLine> _cart = new();

    public OrderLogic(MenuLogic menuLogic)
    {
        _menuLogic = menuLogic;
    }

    public IEnumerable<MenuModel> GetAllMenuItems() => _menuLogic.GetAll();
    public IReadOnlyList<CartLine> GetCart() => _cart;

    public string AddToCart(int menuId, int quantity)
    {
        if (quantity <= 0) return "Quantity must be at least 1.";
        var item = _menuLogic.GetAll().FirstOrDefault(m => m.ID == menuId);
        if (item == null) return $"Menu item with ID {menuId} not found.";
        var existing = _cart.FirstOrDefault(c => c.Item.ID == menuId);
        if (existing == null)
        {
            _cart.Add(new CartLine(item, quantity));
        }
        else
        {
            existing.Quantity += quantity;
        }
        return $"Added {quantity} × \"{CartLine.BuildLabel(item)}\" to cart.";
    }

    public string RemoveFromCart(int menuId)
    {
        var existing = _cart.FirstOrDefault(c => c.Item.ID == menuId);
        if (existing == null) return $"Item with ID {menuId} is not in the cart.";
        _cart.Remove(existing);
        return "Item removed from cart.";
    }

    public string UpdateQuantity(int menuId, int quantity)
    {
        if (quantity <= 0) return "Quantity must be at least 1.";
        var existing = _cart.FirstOrDefault(c => c.Item.ID == menuId);
        if (existing == null) return $"Item with ID {menuId} is not in the cart.";
        existing.Quantity = quantity;
        return "Quantity updated.";
    }

    public double GetTotal() => _cart.Sum(c => c.LineTotal);
    public bool IsCartEmpty() => !_cart.Any();

    public OrderSummary FinalizeOrder()
    {
        var lines = _cart.Select(c => new OrderLineSnapshot(
            c.Item.ID,
            c.Item.Food,
            c.Item.Drink,
            c.Item.Price,
            c.Quantity
        )).ToList();

        var total = lines.Sum(l => l.Price * l.Quantity);
        var orderTime = DateTime.Now;
        var pickupTime = orderTime.AddMinutes(30); 
        var summary = new OrderSummary(orderTime, pickupTime, lines, total);
        _cart.Clear();
        return summary;
    }
}

public class CartLine
{
    public MenuModel Item { get; }
    public int Quantity { get; set; }
    public double LineTotal => Item.Price * Quantity;

    public CartLine(MenuModel item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public static string BuildLabel(MenuModel m)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(m.Food)) parts.Add(m.Food!);
        if (!string.IsNullOrWhiteSpace(m.Drink)) parts.Add(m.Drink!);
        return parts.Count > 0 ? string.Join(" / ", parts) : "(Unnamed)";
    }
}

public record OrderLineSnapshot(int ID, string Food, string Drink, double Price, int Quantity)
{
    public string Label => CartLine.BuildLabel(new MenuModel(ID, Food, Drink, Price));
    public double Subtotal => Price * Quantity;
}

public record OrderSummary(DateTime Timestamp, DateTime PickupTime, List<OrderLineSnapshot> Lines, double Total);
