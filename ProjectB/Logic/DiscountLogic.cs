using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.BLL
{
    public static class DiscountLogic
    {
        public static DiscountSummaryDto ApplyAllDiscounts(
            List<(int sessionId, int age)> cart,
            UserModel? customer,
            string? promoCode = null)
        {
            var offers = OfferRepository.GetActiveOffers();


            var applicable = offers
                .Where(o => IsApplicable(o, cart, customer, promoCode))
                .OrderByDescending(o => o.Discount)
                .ToList();

            var details = new List<TicketDiscountDto>();
            decimal originalSubTotal = 0;

            foreach (var (sessionId, age) in cart)
            {
                var basePrice = 15m;
                originalSubTotal += basePrice;

                var applied = new List<AppliedOfferDto>();
                var price = basePrice;

                foreach (var offer in applicable)
                {
                    if (!IsOfferValidForTicket(offer, age, customer, cart.Count)) continue;

                    var percent = offer.Discount;
                    var amount  = price * percent;
                    applied.Add(new AppliedOfferDto(
                        offer.Id, offer.Nr, offer.Name, percent, amount));

                    price -= amount;
                    if (price <= 0) break;
                }

                details.Add(new TicketDiscountDto(
                    sessionId, age, basePrice, Math.Max(0, price), applied));
            }

            foreach (var promo in applicable.OfType<OfferPromoCode>())
                OfferRepository.IncrementPromoUse(promo.Id);

            return new DiscountSummaryDto(
                details,
                originalSubTotal,
                details.Sum(t => t.FinalPrice));
        }

        private static bool IsApplicable(OfferBase offer, List<(int, int)> cart, UserModel? customer, string? promoCode)
        {
            if (offer is OfferVIP && customer?.Level != UserLevel.VIP) return false;

            if (offer is OfferGroup og)
            {
                var cnt = cart.Count;
                if (og.GroupType == GroupType.School && cnt < 20) return false;
                if (og.GroupType == GroupType.Company  && cnt < 10) return false;
            }

            if (offer is OfferPromoCode pc)
            {
                if (!string.Equals(pc.PromoCode, promoCode, StringComparison.OrdinalIgnoreCase)) return false;
                if (pc.MaxUses.HasValue && pc.CurrentUses >= pc.MaxUses.Value) return false;
            }

            return true;
        }

        private static bool IsOfferValidForTicket(OfferBase offer, int age, UserModel? customer, int cartCount)
        {
            if (offer.Min.HasValue && age < offer.Min.Value) return false;
            if (offer.Max.HasValue && age > offer.Max.Value) return false;
            if (offer.Min.HasValue && cartCount < offer.Min.Value) return false;
            if (offer.Max.HasValue && cartCount > offer.Max.Value) return false;
            return true;
        }
    }
}