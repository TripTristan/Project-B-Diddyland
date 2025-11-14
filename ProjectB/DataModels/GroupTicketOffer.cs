public class GroupTicketOffer : OfferModel
{
    public int MinimumGroupSize { get; private set; }

    public int? MaximumGroupSize { get; private set; }

    public decimal GroupDiscount { get; private set; }

    public bool AllowPartialRefund { get; private set; }

    public bool RequiresAdvanceBooking { get; private set; }

    public int? AdvanceBookingDays { get; private set; }

    public bool IncludesGuideService { get; private set; }

    public string ContactRequirements { get; private set; }

    public GroupTicketOffer(
        int id,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        int minimumGroupSize,
        int? maximumGroupSize = null,
        decimal groupDiscount = 0m,
        bool isActive = false,
        bool targetOnlyOnlineLoginCustomers = false,
        bool allowPartialRefund = false,
        bool requiresAdvanceBooking = false,
        int? advanceBookingDays = null,
        bool includesGuideService = false,
        string contactRequirements = "",
        List<OfferRuleModel>? rules = null,
        int? daysBeforeExpiry = null)
        : base(
            id,
            name,
            description,
            discount,
            startDate,
            endDate,
            isActive,
            targetOnlyOnlineLoginCustomers,
            rules ?? new List<OfferRuleModel>(),
            daysBeforeExpiry)
    {
        if (minimumGroupSize <= 1)
            throw new ArgumentException("Minimum group size must be greater than 1", nameof(minimumGroupSize));

        if (maximumGroupSize.HasValue && maximumGroupSize < minimumGroupSize)
            throw new ArgumentException("Maximum group size cannot be less than minimum group size", nameof(maximumGroupSize));

        this.MinimumGroupSize = minimumGroupSize;
        this.MaximumGroupSize = maximumGroupSize;
        this.GroupDiscount = groupDiscount;
        this.AllowPartialRefund = allowPartialRefund;
        this.RequiresAdvanceBooking = requiresAdvanceBooking;
        this.AdvanceBookingDays = requiresAdvanceBooking ? advanceBookingDays ?? 1 : null;
        this.IncludesGuideService = includesGuideService;
        this.ContactRequirements = contactRequirements ?? string.Empty;
    }
}