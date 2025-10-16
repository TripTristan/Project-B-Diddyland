public class Session
{
    public int Id { get; set; }
    public string Date { get; set; } = "";   
    public string Time { get; set; } = "";
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }

    public decimal PricePerPerson { get; set; }  
}
