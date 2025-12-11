public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public long SessionId { get; set; }
    public int Quantity { get; set; }
    public int CustomerID { get; set; }
    public long BookingDate { get; set; }
    public double Price { get; set; } 
    public int FastPass { get; set; } 

    public ReservationModel(string orderNo, long sessionId, int quantity, UserModel user, long date, double original, int fast)
    {
        OrderNumber = orderNo;
        SessionId = sessionId;
        Quantity = quantity;
        CustomerID = user.Id;
        BookingDate = date;
        Price = original;
        FastPass = fast;
    }

    public ReservationModel() { }

}
