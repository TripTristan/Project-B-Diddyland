public static class MenuLogic
{

    public static IEnumerable<MenuModel> GetAll() => MenusAccess.GetAll();

    public static string AddItem(string? food, string? drink, double price)
    {
        food ??= "";        // Set defaullt value
        drink ??= "";

        if (string.IsNullOrWhiteSpace(food) && string.IsNullOrWhiteSpace(drink))        // Nothing filled in
            return "Please provide at least a food or drink name.";

        if (price < 0)      // Negative price
            return "Price cannot be negative.";

        var model = new MenuModel
        {
            Food = food.Trim(),
            Drink = drink.Trim(),
            Price = price
        };

        MenusAccess.Insert(model);
        return "Item added successfully!";
    }

    public static string AddFood(string name, double price) => AddItem(name, "", price);
    public static string AddDrink(string name, double price) => AddItem("", name, price);

    public static string RemoveItem(int menuId)
    {
        var existing = MenusAccess.GetById(menuId);
        if (existing == null)       // Item not found
            return $"Menu item with ID {menuId} not found.";

        MenusAccess.Delete(menuId);
        return "Item removed successfully.";
    }

}
