public class Menu
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int ItemID { get; set; }
    public string Food { get; set; } = "";
    public string Drink { get; set; } = "";
    public double Price { get; set; }


    public Menu(int itemId, string food, string drink, double price)
    {
        ItemID = itemId;
        Food = food;
        Drink = drink;
        Price = price;
    }   
}