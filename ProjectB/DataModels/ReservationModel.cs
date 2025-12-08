public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public int CustomerID { get; set; }
<<<<<<< HEAD
    public long BookingDate { get; set; }
    public double OriginalPrice { get; set; }
    public double Discount { get; set; }
    public double FinalPrice { get; set; }

    public ReservationModel() { }  // For Dapper

}
=======
    public string BookingDate { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public string Type { get; set; } = "Reservation";

    
    public ReservationModel(string ord, int ses, int qty, UserModel cus, DateTime bok, decimal price, decimal discount, decimal final, string type)
    {
        OrderNumber = ord;
        SessionId = ses;
        Quantity = qty;
        CustomerID = cus.Id;
        BookingDate = bok.ToString("yyyy-MM-dd HH:mm:ss");
        OriginalPrice = price;
        Discount = discount;
        FinalPrice = final;
        Type = type;
    }

}
>>>>>>> main
