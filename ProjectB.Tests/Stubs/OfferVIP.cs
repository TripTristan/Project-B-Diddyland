public class OfferVIP : OfferBase
{
    public OfferVIP(
        string nr,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        int? daysBeforeExpiry)
        : base(nr, name, description, discount, startDate, endDate, 0, 0, isActive, daysBeforeExpiry)
    {
    }
}

