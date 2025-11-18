public class OfferVIP : OfferBase
{
    // OfferVip Table # Database 
    // ..........................
    // Offer only for VIP Customers
    //............................

    public OfferVIP(
        string nr,
        string name, 
        string description, 
        decimal discount, 
        DateTime startDate, 
        DateTime endDate, 
        bool isActive, 
        bool targetOnlyOnlineLoginCustomers, 
        int? daysBeforeExpiry) 
        : base(
            nr,
            name, 
            description, 
            discount, 
            startDate, 
            endDate, 
            isActive, 
            targetOnlyOnlineLoginCustomers, 
            daysBeforeExpiry)
    {
    }
}