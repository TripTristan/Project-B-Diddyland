public class OfferRuleModel
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public string RuleType { get; set; } = ""; 
    public string RuleValue { get; set; } = "";
    public string Description { get; set; } = "";

    // RuleValue te convert to object
    // RuleType = "Quantity", IntValue 
    // RuleType = "PromoCode", StringValue
    // RuleType = "Birthday", DateTimeValue
    public T GetRuleValue<T>() where T : class
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(RuleValue);
        }
        catch
        {
            return null;
        }
    }

    public void SetRuleValue<T>(T value)
    {
        RuleValue = JsonConvert.SerializeObject(value);
    }
}
