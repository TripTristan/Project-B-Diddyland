public class Session
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string Date { get; set; } = "";   
    public SessionTime Time { get; set; }
    public decimal BasisPrice { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentBookings { get; set; }
    public bool IsActive { get; set; }

    public Session(string date, 
                    SessionTime time, 
                    decimal basisPrice, 
                    int maxCapacity, 
                    int currentBookings, 
                    bool isActive)
    {
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
