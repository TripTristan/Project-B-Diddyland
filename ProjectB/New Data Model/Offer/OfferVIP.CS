public class OfferVIP : OfferBase
{
    // OfferVip Table in Database 
    // ..........................
    // Offer only for VIP Customers
    //............................

    public OfferVIP(
        int id, 
        string name, 
        string description, 
        decimal discount, 
        DateTime startDate, 
        DateTime endDate, 
        bool isActive, 
        bool targetOnlyOnlineLoginCustomers, 
        int? daysBeforeExpiry) 
        : base(
            id, 
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