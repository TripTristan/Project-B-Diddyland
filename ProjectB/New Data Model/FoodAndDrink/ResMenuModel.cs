public class ResMenu
{
    public int ID { get; set; } // Primary Key// Database Generated
    public string Nr { get; set; }
    public int MenuID { get; set; }
    public int Quantity { get; set; }


    public ResMenuModel( string nr, int menuID, int quantity)
    {
        Nr = nr;
        MenuID = menuID;
        Quantity = quantity;
    }

}