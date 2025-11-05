public class OfferRuleModel
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public string RuleType { get; set; } = "Quantity";
    public int RuleValue { get; set; } // 
}
