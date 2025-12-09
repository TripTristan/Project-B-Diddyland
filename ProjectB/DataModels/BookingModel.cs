public class BookingModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public string BookingDate { get; set; } = "";
    public decimal Price { get; set; } 
    public long? CustomerId { get; set; }
    public string Type { get; set; } = "Reservation"; 
}
