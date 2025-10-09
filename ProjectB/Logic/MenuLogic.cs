public class MenuLogic
{
    private readonly MenusAccess _access;

    public MenuLogic(MenusAccess access)
    {
        _access = access;
    }

    public IEnumerable<MenuModel> GetAll() => _access.GetAll();

    public void AddItem(string? food, string? drink, double price)
    {
        food ??= "";        // Set Defaullt Value
        drink ??= "";       

        if (string.IsNullOrWhiteSpace(food) && string.IsNullOrWhiteSpace(drink))
            throw new ArgumentException("Provide at least a food or a drink name.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");

        var model = new MenuModel
        {
            Food = food.Trim(),
            Drink = drink.Trim(),
            Price = price
        };

        _access.Insert(model);
    }


    public void AddFood(string name, double price) => AddItem(name, "", price);
    public void AddDrink(string name, double price) => AddItem("", name, price);

}
