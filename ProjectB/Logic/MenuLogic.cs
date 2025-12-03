using System;
using System.Collections.Generic;
using System.Linq;

public class MenuLogic
{
    private readonly MenusAccess _menusAccess;

    public MenuLogic(MenusAccess menusAccess)
    {
        _menusAccess = menusAccess;
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
