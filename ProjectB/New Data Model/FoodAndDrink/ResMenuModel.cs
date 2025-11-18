public class ResMenuModel
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int ReservationID { get; set; }
    public int MenuID { get; set; }
    public int Quantity { get; set; }


    public ResMenuModel( int reservationID, int menuID, int quantity)
    {
        ReservationID = reservationID;
        MenuID = menuID;
        Quantity = quantity;
    }

}