public class Session
{
    public int Id { get; set; }            
    public string Date { get; set; } = "";   
    public string Time { get; set; } = "";    
    public int AttractionID { get; set; }
    public int CurrentBookings { get; set; }  // maps from SQL: Currentbooking AS CurrentBookings
    public string Location { get; set; } = "";
}
