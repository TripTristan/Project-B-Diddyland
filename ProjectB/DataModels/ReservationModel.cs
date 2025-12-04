public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public int CustomerID { get; set; }
    public long BookingDate { get; set; }
    public double OriginalPrice { get; set; }
    public double Discount { get; set; }
    public double FinalPrice { get; set; }

    public ReservationModel() { }  // For Dapper

}
