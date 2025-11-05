public static class OfferEngine
{
    public static decimal ApplyOffers(IEnumerable<OfferModel> allOffers,
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
            if (new OfferAccess().UsageExists(offer.Id, orderNumber)) continue;

            bool ok = true;
            foreach (var r in offer.Rules)
            {
                if (r.RuleType == "Quantity" && ticketQuantity < r.RuleValue)
                { ok = false; break; }
            }
            if (!ok) continue;

            bestDiscount = Math.Max(bestDiscount, offer.Discount);
        }

        return bestDiscount;
    }
}