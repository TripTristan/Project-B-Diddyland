public class OfferVipCustomerModel : OfferBaseCustomerModel
{
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
        decimal vipExclusiveDiscount )
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
              daysBeforeExpiry )
    {}

}
    
    