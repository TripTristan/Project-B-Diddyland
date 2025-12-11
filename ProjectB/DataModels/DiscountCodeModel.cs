public class DiscountCodeModel
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public double Percentage { get; set; }
    public int Active { get; set; }
}
