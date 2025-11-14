public class OfferVipCustomerModel : OfferBaseCustomerModel
{
    public int FreeTicketAfterTickets { get; private set; } = 5; 
    public OfferVipCustomerModel(
        int id,
        string name,
        string description,
        decimal discount,
        DateTime startDate,
        DateTime endDate,
        bool isActive,
        bool targetOnlyOnlineLoginCustomers,
        List<OfferRuleModel> rules,
        int daysBeforeExpiry,
        decimal vipExclusiveDiscount,
        int freeTicketAfterTickets )
        : base(
              id,
              name,
              description,
              discount,
              startDate,
              endDate,
              isActive,
              targetOnlyOnlineLoginCustomers,
              rules,
              daysBeforeExpiry)
    {
        FreeTicketAfterTickets = freeTicketAfterTickets;
    }

}
    
    