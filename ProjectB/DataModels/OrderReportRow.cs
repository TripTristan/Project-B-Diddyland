public class OrderReportRow
{
    public string OrderNumber { get; set; } = "";
    public DateTime BookingDate { get; set; }
    public int CustomerId { get; set; }
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
}
