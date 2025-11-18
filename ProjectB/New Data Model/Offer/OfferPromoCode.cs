public class OfferPromoCode
{
    public int Id { get; set; }
    public string PromoCodeNr { get; set; }
    public string PromoCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public decimal Discount { get; set; }

    public OfferPromoCode(
        string promoCodeNr,//PromoCodeNr
        string promoCode,
        DateTime? expiryDate,
        int? maxUses,
        int currentUses,
        decimal discount)
    {
        PromoCodeNr = promoCodeNr;
        PromoCode = promoCode;
        ExpiryDate = expiryDate;
        MaxUses = maxUses;
        CurrentUses = currentUses;
        Discount = discount;
    }


}