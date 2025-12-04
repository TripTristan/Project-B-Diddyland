public class BookingModel
{
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
}
