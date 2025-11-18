public class ResMenu
{
    public int ID { get; set; } // Primary Key// Database Generated
    public string Nr { get; set; }
    public int MenuID { get; set; }
    public int Quantity { get; set; }


<<<<<<< HEAD
    public ResMenu(string nr, int menuID, int quantity)
=======
    public ResMenuModel( string nr, int menuID, int quantity)
>>>>>>> 779559d6e12ade193080a3483b9436de0ab5f535
    {
        Nr = nr;
        MenuID = menuID;
        Quantity = quantity;
    }

}