using System;
using System.Collections.Generic;
using System.Linq;

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
