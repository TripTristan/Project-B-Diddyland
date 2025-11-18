using System;
using System.Collections.Generic;
using System.Linq;
using ProjectB.DataModels;
using ProjectB.Customer.DataAccess;

namespace ProjectB.Services
{
    public class DiscountService
    {
        private readonly OfferAccess _offerAccess;

        public DiscountService(OfferAccess offerAccess)
        {
            _offerAccess = offerAccess ?? throw new ArgumentNullException(nameof(offerAccess));
        }

        public decimal CalculateTotalDiscount(
            List<TicketOrder> tickets,
            string promoCode,
            UserModel customer,
            string orderNumber)
        {
            if (tickets == null || !tickets.Any())
                return 0m;

            var offers = _offerAccess.GetAllActiveOffers(DateTime.Now).Result.ToList();

            if (!offers.Any())
            {
                return 0m;
            }
        

            decimal totalDiscount = 0m;
            decimal subtotal = tickets.Sum(t => t.Price);

            foreach (var ticket in tickets)
            {
                var ageDiscount = CalculateAgeDiscount(ticket.Age, offers);
                ticket.Discount = ageDiscount * ticket.Price;
            }

            var quantityDiscount = CalculateQuantityDiscount(tickets.Count, offers);
            totalDiscount += quantityDiscount * subtotal;

            if (!string.IsNullOrEmpty(promoCode))
            {
                var promoDiscount = CalculatePromoCodeDiscount(promoCode, customer, orderNumber, offers);
                totalDiscount += promoDiscount * (subtotal - tickets.Sum(t => t.Discount));
            }

            if (customer != null && !string.IsNullOrEmpty(customer.DateOfBirth))
            {
                if (DateTime.TryParseExact(customer.DateOfBirth, "dd-MM-yyyy", 
                    null, System.Globalization.DateTimeStyles.None, out var birthDate))
                {
                    var birthdayDiscount = CalculateBirthdayDiscount(birthDate, offers);
                    totalDiscount += birthdayDiscount * (subtotal - tickets.Sum(t => t.Discount));
                }
            }

            return totalDiscount;
        }

        private decimal CalculateAgeDiscount(int age, List<OfferModel> offers)
        {
            var ageOffers = offers.Where(o => o.Rules.Any(r => r.RuleType == RuleType.Age));
            
            foreach (var offer in ageOffers)
            {
                foreach (var rule in offer.Rules.Where(r => r.RuleType == RuleType.Age))
                {
                    var parameters = rule.GetRuleValue<AgeRuleParameters>();
                    if (parameters != null)
                    {
                        if ((!parameters.MinAge.HasValue || age >= parameters.MinAge) &&
                            (!parameters.MaxAge.HasValue || age <= parameters.MaxAge))
                        {
                            return offer.Discount;
                        }
                    }
                }
            }
            return 0m;
        }

        private decimal CalculateQuantityDiscount(int ticketCount, List<OfferModel> offers)
        {
            var quantityOffers = offers.Where(o => o.Rules.Any(r => r.RuleType == RuleType.Quantity));
            
            foreach (var offer in quantityOffers.OrderByDescending(o => o.Discount))
            {
                foreach (var rule in offer.Rules.Where(r => r.RuleType == RuleType.Quantity))
                {
                    var parameters = rule.GetRuleValue<QuantityRuleParameters>();
                    if (parameters != null && ticketCount >= parameters.MinQuantity)
                    {
                        return offer.Discount;
                    }
                }
            }
            return 0m;
        }

        private decimal CalculatePromoCodeDiscount(
            string promoCode, 
            UserModel customer, 
            string orderNumber, 
            List<OfferModel> offers)
        {
            var promoOffers = offers.Where(o => o.Rules.Any(r => r.RuleType == RuleType.PromoCode));
            
            foreach (var offer in promoOffers)
            {
                foreach (var rule in offer.Rules.Where(r => r.RuleType == RuleType.PromoCode))
                {
                    var parameters = rule.GetRuleValue<PromoCodeRuleParameters>();
                    if (parameters != null && 
                        string.Equals(parameters.Code, promoCode, StringComparison.OrdinalIgnoreCase))
                    {
                        if (parameters.ExpiryDate.HasValue && parameters.ExpiryDate.Value < DateTime.Now)
                            continue;
                            
                        if (parameters.MaxUses.HasValue && parameters.CurrentUses >= parameters.MaxUses.Value)
                            continue;
                            
                        if (customer != null && _offerAccess.IsPromoCodeUsed(promoCode, customer.Id).Result)
                            continue;
                            
                        _offerAccess.RecordOfferUsageAsync(new OfferUsageModel
                        {
                            OfferId = offer.Id,
                            OrderNumber = orderNumber,
                            CustomerId = customer?.Id,
                            UsedAt = DateTime.Now
                        }).Wait();
                        
                        _offerAccess.IncrementPromoCodeUsage(offer.Id, promoCode).Wait();
                        
                        return offer.Discount;
                    }
                }
            }
            
            return 0m;
        }

        private decimal CalculateBirthdayDiscount(DateTime birthDate, List<OfferModel> offers)
        {
            var today = DateTime.Today;
            var thisYearBirthday = new DateTime(today.Year, birthDate.Month, birthDate.Day);
            
            if (thisYearBirthday < today)
                thisYearBirthday = thisYearBirthday.AddYears(1);
                
            var birthdayWindowStart = thisYearBirthday.AddDays(-7);
            var birthdayWindowEnd = thisYearBirthday.AddDays(7);
            
            if (today >= birthdayWindowStart && today <= birthdayWindowEnd)
            {
                var birthdayOffers = offers.Where(o => o.Rules.Any(r => r.RuleType == RuleType.Birthday));
                return birthdayOffers.Max(o => o.Discount);
            }
            
            return 0m;
        }
    }
}
