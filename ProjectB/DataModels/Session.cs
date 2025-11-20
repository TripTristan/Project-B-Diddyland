public class Session
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string Nr{ get; set; }
    public string Date { get; set; }  
    public string Time { get; set; } = "";
    public int AttractionID { get; set; }
    public decimal BasisPrice { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }
    public bool IsActive { get; set; }


    public Session(string nr, 
                    string date, 
                    string time, 
                    decimal basisPrice, 
                    int maxCapacity, 
                    int currentBookings, 
                    bool isActive)
    {
        Nr = nr;
        Date = date;
        Time = time;
        BasisPrice = basisPrice;
        MaxCapacity = maxCapacity;
        CurrentBookings = currentBookings;
        IsActive = isActive;
        ValiCurrentBooingAndMaxCapacity();
    }


    private void ValiCurrentBooingAndMaxCapacity()
    {
        if (CurrentBookings > MaxCapacity)
        {
            throw new ArgumentException("Current bookings cannot exceed maximum capacity.");
        }
    }

}
