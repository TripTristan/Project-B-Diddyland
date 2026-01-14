public class SessionModel
{
    
    public long Id { get; set; }            
    public long Date { get; set; }   
    public long Time { get; set; }
    public long Capacity { get; set; }
    public int AttractionId { get; set; }
    public string Location { get; set; } = "";
    public int SessionType { get; set; } 

    public SessionModel() { }

    public SessionModel(long id, long date, long time, long capacity)
    {
        Id =  id; 
        Date = date;
        Time = time;
        Capacity = capacity;
    }  
}
