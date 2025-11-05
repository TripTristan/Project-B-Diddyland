public class OfferUsageModel
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public string OrderNumber { get; set; } = "";
    public int? CustomerId { get; set; }
    public DateTime UsedAt { get; set; }
}