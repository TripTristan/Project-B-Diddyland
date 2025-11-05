public class OfferModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool TargetOnlyCustomers { get; set; } = true;

    public List<OfferRuleModel> Rules { get; set; } = new();
}