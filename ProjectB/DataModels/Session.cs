public class Session
{
    public int Id { get; set; }               // maps from SQL: ID AS Id
    public string Date { get; set; } = "";    // "yyyy-MM-dd"
    public string Time { get; set; } = "";    // "HH:mm"
    public int AttractionID { get; set; }
    public int CurrentBookings { get; set; }  // maps from SQL: Currentbooking AS CurrentBookings
}
