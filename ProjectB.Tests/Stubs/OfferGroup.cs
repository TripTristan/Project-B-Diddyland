public class OfferGroup : OfferBase
{
    public GroupType GroupType { get; set; }

    public OfferGroup(
        string nr,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        int? daysBeforeExpiry,
        int? max,
        int? min,
        GroupType groupType)
        : base(nr, name, description, discount, startDate, endDate, max ?? 0, min ?? 0, isActive, daysBeforeExpiry)
    {
        GroupType = groupType;
    }
}

