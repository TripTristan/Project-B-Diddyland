public class MenuModel
{
    public int MenuID { get; set; }
    public string Food { get; set; } = "";
    public string Drink { get; set; } = "";
    public double Price { get; set; }

    public MenuModel() { }
    public MenuModel(int menuId, string food, string drink, double price)
    {
        MenuID = menuId;
        Food = food;
        Drink = drink;
        Price = price;
    }   
}