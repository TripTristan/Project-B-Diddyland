public class OfferBase
{
    // Offer Table # Database 
    // ..........................
    // Offer only for Regular Customers and free Customer
    //............................
    
    public int Id { get; set; } // Primary Key// Database Generated
    public string OfferNr { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? Max { get; set; } = 0; // Age or Quantity
    public int? Min { get; set; } =0; // Age or Quantity
    public DateTime CreatedAt { get; set; }
    public int? DaysBeforeExpiry { get; set; } = 0;
    public bool IsActive { get; set; } = false;


    public OfferBase( 
        string offerNr,
        string name, 
        string description, 
        decimal discount, 
        DateTime startDate, 
        DateTime endDate, 
        int max, 
        int min, 
        bool isActive, 
        int? daysBeforeExpiry)
    {
        OfferNr = offerNr;
        Name = name;
        Description = description ?? "";
        Discount = discount;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        Max = max;
        Min = min;
        DaysBeforeExpiry = daysBeforeExpiry;
        CreatedAt = DateTime.Now;
        ValidateDates();
    }


    // no startDate and endDate
    // Start = Now, End = 100 years later ......................./
    public OfferBase( 
        string name, 
        string description, 
        decimal discount, 
        int max, 
        int min, 
        bool isActive, 
        int? daysBeforeExpiry)
    {
        Name = name;
        Description = description ?? "";
        Discount = discount;

        StartDate = DateTime.Now;
        EndDate = DateTime.Now.AddYears(100);

        
        Max = max;
        Min = min;
        IsActive = isActive;

        DaysBeforeExpiry = daysBeforeExpiry;
        CreatedAt = DateTime.Now;
        ValidateDates();
    }


    private void ValidateDates()
    {
        if (EndDate <= StartDate)
        {
            throw new ArgumentException("StartDate must be before EndDate");
        }
    }

    private void VailMaxMin()
    {
        if ( Min > Max)
        {
            throw new ArgumentException("Min cannot be greater than Max");
        }
    }
}