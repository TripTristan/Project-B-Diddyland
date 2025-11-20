public class BookingModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public string BookingDate { get; set; } = "";
    public decimal OriginalPrice { get; set; } 
    public decimal Discount { get; set; }     
    public decimal FinalPrice { get; set; }    
    public long? CustomerId { get; set; }
}
