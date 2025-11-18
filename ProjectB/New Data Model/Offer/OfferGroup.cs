public class OfferGroup : OfferBase
{
    // OfferGroup Table # Database 
    // ..........................
    // Offer Group for Regular Customers and free Customer
    //............................

    public GroupType GroupType { get; set; }

     public OfferVIP(
        string offerNr,
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
        : base(
            offerNr,
            name, 
            description, 
            discount, 
            startDate, 
            endDate, 
            isActive, 
            daysBeforeExpiry,
            max,
            min)
    {
        GroupType = groupType;
    }
    
}