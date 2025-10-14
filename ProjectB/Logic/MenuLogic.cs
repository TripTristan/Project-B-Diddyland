public class MenuLogic
{
    private readonly MenusAccess _access;

    public MenuLogic(MenusAccess access)
    {
        _access = access;
    }

    public IEnumerable<MenuModel> GetAll() => _access.GetAll();

    public string AddItem(string? food, string? drink, double price)
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

        _access.Insert(model);
        return "Item added successfully!";
    }

    public string AddFood(string name, double price) => AddItem(name, "", price);
    public string AddDrink(string name, double price) => AddItem("", name, price);

    public string RemoveItem(int menuId)
    {
        var existing = _access.GetById(menuId);
        if (existing == null)       // Item not found
            return $"Menu item with ID {menuId} not found.";

        _access.Delete(menuId);
        return "Item removed successfully.";
    }

}
