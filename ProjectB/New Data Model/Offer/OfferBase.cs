public class OfferBase
{
    // Offer Table in Database 
    // ..........................
    // Offer only for Regular Customers and free Customer
    //............................
    
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
    public bool TargetOnlyOnlineLoginCustomers { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public int? DaysBeforeExpiry { get; set; } = 0;


    public OfferModel(
        int id,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        bool targetOnlyOnlineLoginCustomers,
        int? daysBeforeExpiry)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? "";
        Discount = discount;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        TargetOnlyOnlineLoginCustomers = targetOnlyOnlineLoginCustomers;
        DaysBeforeExpiry = daysBeforeExpiry;
        CreatedAt = DateTime.Now;
        ValidateDates();
    }


    // no startDate and endDate
    // Start = Now, End = 100 years later .......................
    public OfferModel(
        int id,
        string name,
        string description,
        decimal discount,
        bool isActive = false,
        bool targetOnlyOnlineLoginCustomers = false,
        int? daysBeforeExpiry = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? "";
        Discount = discount;
        StartDate = DateTime.Now;
        EndDate = DateTime.Now.AddYears(100);
        IsActive = isActive;
        TargetOnlyOnlineLoginCustomers = targetOnlyOnlineLoginCustomers;
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
}