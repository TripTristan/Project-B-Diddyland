public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public UserModel? Customer { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public bool PaymentStatus { get; set; } = false;

    public ReservationModel(
        string orderNumber,
        int sessionId,
        int quantity,
        UserModel? customer,
        DateTime bookingDate,
        decimal originalPrice,
        decimal discount,
        decimal finalPrice,
        bool paymentStatus){
        OrderNumber = orderNumber;
        SessionId = sessionId;
        Quantity = quantity;
        Customer = customer;
        BookingDate = bookingDate;
        OriginalPrice = originalPrice;
        Discount = discount;
        FinalPrice = finalPrice;
        PaymentStatus = paymentStatus;
    }

}
