using System;
using System.Collections.Generic;
using System.Linq;
using MyProject.DAL;

namespace MyProject.BLL
{
    public static class DiscountLogic
    {
        public static DiscountSummaryDto ApplyAllDiscounts(
            List<(int sessionId, int age)> cart,
            UserModel? customer,
            string? promoCode = null,
            DateTime? bookingDate = null)
        {
            var offers = OfferAccess.GetActiveOffers();


            var applicable = offers
                .Where(o => IsApplicable(o, cart, customer, promoCode))
                .OrderByDescending(o => o.Discount)
                .ToList();

            bool canUseBirthdayTicket = false;
            if (customer != null && bookingDate.HasValue)
            {
                canUseBirthdayTicket = CheckBirthdayDiscount(customer, bookingDate.Value, cart) == true;
            }

            var details = new List<TicketDiscountDto>();
            decimal originalSubTotal = 0;
            bool birthdayTicketApplied = false;

            foreach (var (sessionId, age) in cart)
            {
                var basePrice = 15m;
                originalSubTotal += basePrice;

                var applied = new List<AppliedOfferDto>();
                var price = basePrice;


                if (canUseBirthdayTicket && !birthdayTicketApplied && details.Count == 0)
                {
                    applied.Add(new AppliedOfferDto(
                        0, "BIRTHDAY", "Birthday Free Ticket", 1.0m, basePrice));
                    price = 0;
                    birthdayTicketApplied = true;
                }
                else
                {
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
                }

                details.Add(new TicketDiscountDto(
                    sessionId, age, basePrice, Math.Max(0, price), applied));
            }

            foreach (var promo in applicable.OfType<OfferPromoCode>())
                OfferAccess.IncrementPromoUse(promo.Id);

            return new DiscountSummaryDto(
                details,
                originalSubTotal,
                details.Sum(t => t.FinalPrice));
        }

        private static bool? CheckBirthdayDiscount(UserModel customer, DateTime bookingDate, List<(int sessionId, int age)> cart)
        {
            if (string.IsNullOrEmpty(customer.DateOfBirth)) return null;

            try
            {
                string[] dobParts = customer.DateOfBirth.Split("-");
                if (dobParts.Length != 3) return null;

                int day = int.Parse(dobParts[0]);
                int month = int.Parse(dobParts[1]);
                int year = int.Parse(dobParts[2]);

                DateTime thisYearBirthday = new DateTime(bookingDate.Year, month, day);
                DateTime lastYearBirthday = new DateTime(bookingDate.Year - 1, month, day);
                DateTime nextYearBirthday = new DateTime(bookingDate.Year + 1, month, day);

                bool isWithinRange = false;
                DateTime relevantBirthday = thisYearBirthday;

                if (Math.Abs((bookingDate.Date - thisYearBirthday.Date).TotalDays) <= 7)
                {
                    isWithinRange = true;
                    relevantBirthday = thisYearBirthday;
                }
                else if (Math.Abs((bookingDate.Date - lastYearBirthday.Date).TotalDays) <= 7)
                {
                    isWithinRange = true;
                    relevantBirthday = lastYearBirthday;
                }
                else if (Math.Abs((bookingDate.Date - nextYearBirthday.Date).TotalDays) <= 7)
                {
                    isWithinRange = true;
                    relevantBirthday = nextYearBirthday;
                }

                if (!isWithinRange) return null;

            
                int currentYear = bookingDate.Year;
                if (BirthdayTicketAccess.HasUsedBirthdayTicket(customer.Id, currentYear))
                    return null;

                return true;
            }
            catch
            {
                return null;
            }
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