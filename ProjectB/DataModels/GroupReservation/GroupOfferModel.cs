public class GroupOfferModel : OfferModel
{
    public int MinimumGroupSize { get; private set; }
    public int? MaximumGroupSize { get; private set; }
    public GroupType GroupType { get; private set; }

    public GroupTicketOffer(
        int id,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        int minimumGroupSize,
        int? maximumGroupSize = null,
        GroupType groupType = GroupType.None,
        bool isActive = false,
        bool targetOnlyOnlineLoginCustomers = false,
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
            throw new ArgumentException("must be groter dan 1", nameof(minimumGroupSize));

        if (maximumGroupSize.HasValue && maximumGroupSize < minimumGroupSize)
            throw new ArgumentException("The maximum value cannot be less than the minimum value.", nameof(maximumGroupSize));

        this.MinimumGroupSize = minimumGroupSize;
        this.MaximumGroupSize = maximumGroupSize;
        this.GroupType = groupType;
    }
}