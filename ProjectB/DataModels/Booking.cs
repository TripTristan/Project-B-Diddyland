public class  Booking
{
    public string CustomerId { get; set; }
    public DateTime BookingDate { get; set; }
    public int BookingId { get; set; }


    public Booking(string customerId)
    {
        CustomerId = customerId;
        BookingDate = DateTime.Now;
        BookingId = "";
    }

    public Booking()
    {
        CustomerId = "Guest";
        BookingDate = DateTime.Now;
        BookingId = "";
    }
    
}