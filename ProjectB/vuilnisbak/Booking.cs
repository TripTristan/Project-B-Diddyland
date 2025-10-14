public class Booking
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int SessionId { get; set; }
    public string BookingDate { get; set; }
    public string Period { get; set; }
    public string CreatedAt { get; set; }

    public Booking(int customerId = 0)
    {
        CustomerId = customerId;
        BookingDate = DateTime.Now.ToString("yyyy-MM-dd");
        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}