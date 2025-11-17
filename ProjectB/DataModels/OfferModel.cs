public class OfferModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Discount { get; set; }
    public bool IsActive { get; set; } = false;
    public bool TargetOnlyOnlineLoginCustomers { get; set; } = false;
    public List<OfferRuleModel> Rules { get; set; } = new();
    public int? DaysBeforeExpiry { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public OfferModel(
        int id,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        bool targetOnlyOnlineLoginCustomers,
        List<OfferRuleModel> rules,
        int? daysBeforeExpiry )
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? "";
        Discount = discount;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        TargetOnlyOnlineLoginCustomers = targetOnlyOnlineLoginCustomers;
        Rules = rules ?? new List<OfferRuleModel>();
        ValidateDates();
        DaysBeforeExpiry = daysBeforeExpiry;
    }

    // IF no startDate and endDate provided, set to default values
    // Start = Now, End = 100 years later .......................
    public OfferModel(
        int id,
        string name,
        string description,
        decimal discount,
        bool isActive = false,
        bool targetOnlyOnlineLoginCustomers = false,
        List<OfferRuleModel>? rules = null,
        int daysBeforeExpiry )
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? "";
        Discount = discount;

        StartDate = DateTime.Now;
        endDate: DateTime.Now.AddYears(100) ; 
        // DateTime.AddYears(100)
        // DateTime.AddDays(36500);
        // 100 years later .......................

        IsActive = isActive;
        TargetOnlyOnlineLoginCustomers = targetOnlyOnlineLoginCustomers;
        Rules = rules ?? new List<OfferRuleModel>();
        DaysBeforeExpiry = daysBeforeExpiry;
        ValidateDates();
    }
}