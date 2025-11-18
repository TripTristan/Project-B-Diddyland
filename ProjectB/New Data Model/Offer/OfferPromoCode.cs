public class OfferPromoCode
{
    public int Id { get; set; }
    public string Nr { get; set; }
    public string PromoCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public decimal Discount { get; set; }

    public OfferPromoCode(
        string nr,//PromoCodeNr
        string promoCode,
        DateTime? expiryDate,
        int? maxUses,
        int currentUses,
        decimal discount)
    {
        Nr = nr;
        PromoCode = promoCode;
        ExpiryDate = expiryDate;
        MaxUses = maxUses;
        CurrentUses = currentUses;
        Discount = discount;
    }


}