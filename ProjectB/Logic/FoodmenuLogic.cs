using System;
using System.Collections.Generic;
using System.Linq;

public class FoodmenuLogic
{
    private MenusAccess _menusAccess;
    public FoodmenuLogic() {_menusAccess = new MenusAccess(new DatabaseContext("Data Source=DataSources/diddyland.db")); }

    private readonly List<CartLine> _cart = new();

    public IEnumerable<MenuModel> GetAllMenuItems() => GetAll();

    public IReadOnlyList<CartLine> GetCart() => _cart;

    public string AddToCart(int menuId, int quantity)
    {
        if (quantity <= 0) return "Quantity must be at least 1.";

        var item = GetAll().FirstOrDefault(m => m.ID == menuId);
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


    public IEnumerable<MenuModel> GetAll() => _menusAccess.GetAll();

    public string AddItem(string? food, string? drink, double price)
    {
        food ??= "";
        drink ??= "";

        if (string.IsNullOrWhiteSpace(food) && string.IsNullOrWhiteSpace(drink))
            return "Please provide at least a food or drink name.";

        if (price < 0)
            return "Price cannot be negative.";

        var existing = _menusAccess.GetAll();

        bool duplicate = existing.Any(m =>
            string.Equals(m.Food ?? "", food.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(m.Drink ?? "", drink.Trim(), StringComparison.OrdinalIgnoreCase)
        );

        if (duplicate)
            return "This menu item already exists.";

        var model = new MenuModel
        {
            Food = food.Trim(),
            Drink = drink.Trim(),
            Price = price
        };

        _menusAccess.Insert(model);
        return "Item added successfully!";
    }

    public string AddFood(string name, double price) => AddItem(name, "", price);

    public string AddDrink(string name, double price) => AddItem("", name, price);

    public string RemoveItem(int menuId)
    {
        var existing = _menusAccess.GetById(menuId);
        if (existing == null)
            return $"Menu item with ID {menuId} not found.";

        _menusAccess.Delete(menuId);
        return "Item removed successfully.";
    }
}
