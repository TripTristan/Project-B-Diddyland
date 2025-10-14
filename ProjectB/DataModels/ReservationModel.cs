public class ReservationModel
{
    public string OrderNumber { get; set; } = "";
    public int SessionId { get; set; }
    public int Quantity { get; set; }
    public UserModel? Customer { get; set; }
    public DateTime BookingDate { get; set; }
}