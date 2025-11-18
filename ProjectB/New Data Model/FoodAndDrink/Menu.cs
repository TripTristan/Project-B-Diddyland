public class Menu
{
    public int ID { get; set; } // Primary Key// Database Generated
    public string Nr { get; set; }
    public string Food { get; set; } = "";
    public string Drink { get; set; } = "";
    public double Price { get; set; }


    public Menu(string nr, string food, string drink, double price)
    {
        Nr = nr;
        Food = food;
        Drink = drink;
        Price = price;
    }   
}