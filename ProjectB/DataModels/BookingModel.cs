public class BookingModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public string BookingDate { get; set; } = "";
    public string OriginalPrice { get; set; } = "";
    public string Discount { get; set; } = "";
    public string FinalPrice { get; set; } = "";

    public long? CustomerId { get; set; }
}
