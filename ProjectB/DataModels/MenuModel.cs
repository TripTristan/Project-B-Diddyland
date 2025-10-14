public class MenuModel
{
    public int ID { get; set; }
    public string Food { get; set; } = "";
    public string Drink { get; set; } = "";
    public double Price { get; set; }

    public MenuModel() { }
    public MenuModel(int id, string food, string drink, double price)
    {
        ID = id;
        Food = food;
        Drink = drink;
        Price = price;
    }   
}