public class OfferPromoCode : OfferBase
{
    public string PromoCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MaxUses { get; set; }
    public int? CurrentUses { get; set; }

    public OfferPromoCode(
        string nr,
        string promoCode,
        DateTime? expiryDate,
        int? maxUses,
        int? currentUses,
        decimal discount)
        : base(nr, promoCode, promoCode, discount, DateTime.Now, DateTime.Now.AddYears(1), 0, 0, true, null)
    {
        PromoCode = promoCode;
        ExpiryDate = expiryDate;
        MaxUses = maxUses;
        CurrentUses = currentUses;
    }
}

