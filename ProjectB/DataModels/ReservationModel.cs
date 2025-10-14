public class ReservationModel
{
    public int ReservationID { get; set; }
    public int AccountID { get; set; }
    public int TicketID { get; set; }
    public double Price { get; set; }
    public string Date { get; set; } = "";
    public int Amount { get; set; }
    public string Time { get; set; } = ""; //morning, afternoon
}