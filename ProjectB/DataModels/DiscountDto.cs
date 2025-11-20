using System.Collections.Generic;

namespace MyProject.BLL
{
    public class AppliedOfferDto
    {
        public int OfferId { get; set; }
        public string OfferNr { get; set; }
        public string OfferName { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }

        public AppliedOfferDto(int offerId, string offerNr, string offerName, decimal discountPercent, decimal discountAmount)
        {
            OfferId = offerId;
            OfferNr = offerNr;
            OfferName = offerName;
            DiscountPercent = discountPercent;
            DiscountAmount = discountAmount;
        }
    }

    public class TicketDiscountDto
    {
        public int SessionId { get; set; }
        public int Age { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public List<AppliedOfferDto> AppliedOffers { get; set; }

        public TicketDiscountDto(int sessionId, int age, decimal basePrice, decimal finalPrice, List<AppliedOfferDto> appliedOffers)
        {
            SessionId = sessionId;
            Age = age;
            BasePrice = basePrice;
            FinalPrice = finalPrice;
            AppliedOffers = appliedOffers;
        }
    }

    public class DiscountSummaryDto
    {
        public List<TicketDiscountDto> TicketDetails { get; set; }
        public decimal OriginalSubTotal { get; set; }
        public decimal FinalTotal { get; set; }

        public DiscountSummaryDto(List<TicketDiscountDto> ticketDetails, decimal originalSubTotal, decimal finalTotal)
        {
            TicketDetails = ticketDetails;
            OriginalSubTotal = originalSubTotal;
            FinalTotal = finalTotal;
        }
    }
}


