public class MenuModel
{
    public int ID { get; set; }
    public string Food { get; set; }
    public string Drink { get; set; }
    public double Price { get; set; }

    public MenuModel(int ID, string Food, string Drink, double Price)
    {
        ID = id;
        Food = food;
        Drink = drink;
        Price = price;
    }
}