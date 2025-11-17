public class OfferGroup : OfferBase
{
    // OfferGroup Table in Database 
    // ..........................
    // Offer Group for Regular Customers and free Customer
    //............................

    public GroupType GroupType { get; set; }

     public OfferVIP(
        int id, 
        string name, 
        string description, 
        decimal discount, 
        DateTime startDate, 
        DateTime endDate, 
        bool isActive, 
        bool targetOnlyOnlineLoginCustomers, 
        int? daysBeforeExpiry,
        GroupType groupType) 
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
        GroupType = groupType;
    }
    
}