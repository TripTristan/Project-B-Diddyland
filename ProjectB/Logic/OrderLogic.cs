public static class OrderLogic
{
    private static readonly List<CartLine> _cart = new();

    public static IEnumerable<MenuModel> GetAllMenuItems() => MenuLogic.GetAll();
    public static IReadOnlyList<CartLine> GetCart() => _cart;

    public static string AddToCart(int menuId, int quantity)
    {
        if (quantity <= 0) return "Quantity must be at least 1.";
        var item = MenuLogic.GetAll().FirstOrDefault(m => m.ID == menuId);
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

    public static string RemoveFromCart(int menuId)
    {
        var existing = _cart.FirstOrDefault(c => c.Item.ID == menuId);
        if (existing == null) return $"Item with ID {menuId} is not in the cart.";
        _cart.Remove(existing);
        return "Item removed from cart.";
    }

    public static string UpdateQuantity(int menuId, int quantity)
    {
        if (quantity <= 0) return "Quantity must be at least 1.";
        var existing = _cart.FirstOrDefault(c => c.Item.ID == menuId);
        if (existing == null) return $"Item with ID {menuId} is not in the cart.";
        existing.Quantity = quantity;
        return "Quantity updated.";
    }

    public static double GetTotal() => _cart.Sum(c => c.LineTotal);
    public static bool IsCartEmpty() => !_cart.Any();

    public static OrderSummary FinalizeOrder()
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

