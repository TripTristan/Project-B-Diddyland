public static class OfferManagementLogic
{
    // now only Rule Types: Quantity and Birthday. 
    // Not age
    public static Dictionary<string, decimal> ApplyOffers(IEnumerable<OfferModel> allOffers,
                                      string orderNumber,
                                      int ticketQuantity,
                                      UserModel? customer)
    {
        decimal bestDiscount = 0m;

        foreach (var offer in allOffers.Where(o => o.IsActive &&
                                                   DateTime.Now >= o.StartDate &&
                                                   DateTime.Now <= o.EndDate))
        {
            if (offer.TargetOnlyCustomers && customer == null) continue;
            // offer.TargetOnlyCustomers , dan only ingeloed Customer
            // beperkt Inlogen or register customers
            if (new OfferAccess().UsageExists(offer.Id, orderNumber)) continue;
            Dictionary<string, decimal> ruleResults = new Dictionary<string, decimal>();

            bool ok = true;
            foreach (var r in offer.Rules)
            {
                switch (r.RuleType)
                {
                    case "Quantity":
                        if (ticketQuantity < r.RuleValue) ok = false;
                        else ruleResults["Quantity"] = r.Discount;
                        break;

                    case "Birthday":
                        if (customerBirthday == null ||
                            customerBirthday.Value.Month != DateTime.Now.Month ||
                            customerBirthday.Value.Day != DateTime.Now.Day)
                            ok = false;
                        else ruleResults["Birthday"] = r.Discount;
                        break;
                }
                if (!ok) break;
            }
            if (!ok) continue;

        }

        return ruleResults;
    }
    

    public static decimal CalculateRuleTypeAageDiscount(int age)
    {
        foreach (var offer in allOffers.Where(o => o.IsActive)
                                            .SelectMany(o => o.Rules)
                                            .Where(r => r.RuleType == "Age"))
            {
                if (age >= offer.RuleValue.StratAge && age <= offer.RuleValue.EndAge)
                {
                    return offer.Discount;
                }
            }
    }
}