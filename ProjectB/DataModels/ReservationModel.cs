public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public int CustomerID { get; set; }
    public string BookingDate { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
    
    public ReservationModel(string ord, int ses, int qty, UserModel cus, DateTime bok, decimal price, decimal discount, decimal final)
    {
        OrderNumber = ord;
        SessionId = ses;
        Quantity = qty;
        CustomerID = cus.Id;
        BookingDate = bok.ToString("ddMMyyyy-HHmm");
        OriginalPrice = price;
        Discount = discount;
        FinalPrice = final;
    }
}