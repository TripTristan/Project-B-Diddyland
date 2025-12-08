public class BookingModel
{
<<<<<<< HEAD
    public string OrderNumber;
    public int SessionId;
    public int Quantity;
    public string BookingDate;
    public string OriginalPrice;
    public string Discount;
    public string FinalPrice;
    public long? CustomerId;

    public BookingModel(string orderNumber, int sessionId, int quantity, string bookingDate, string originalPrice, string discount, string finalPrice, long? customerId)
    {
        OrderNumber = orderNumber;
        SessionId = sessionId;
        Quantity = quantity;
        BookingDate = bookingDate;
        OriginalPrice = originalPrice;
        Discount = discount;
        FinalPrice = finalPrice;
        CustomerId = customerId;
    }
=======
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public string BookingDate { get; set; } = "";
    public decimal OriginalPrice { get; set; } 
    public decimal Discount { get; set; }     
    public decimal FinalPrice { get; set; }    
    public long? CustomerId { get; set; }
    public string Type { get; set; } = "Reservation"; 
>>>>>>> main
}
