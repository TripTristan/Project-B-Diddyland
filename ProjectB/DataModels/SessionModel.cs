public class SessionModel
{
    
    public long Id { get; set; }            
    public long Date { get; set; }   
    public long Time { get; set; }
    public long ParkID { get; set; }
    public long VIPCapacity { get; set; }
    public long Capacity { get; set; }

    public SessionModel(long id, long date, long time, long capacity, long VIPcapacity, long parkid)
    {
        Id =  id; 
        Date = date;
        Time = time;
        ParkID = parkID;
        VIPCapacity = VIPcapacity;
        Capacity = capacity;
    }  
}
