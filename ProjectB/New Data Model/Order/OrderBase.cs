public class OrderBase
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Subtotal { get; set; }
    
    public OrderBase(
        int id,
        string orderNumber,
        int customerId,
        DateTime orderDate,
        decimal subtotal)
    {
        Id = id;
        OrderNumber = orderNumber;
        CustomerId = customerId;
        OrderDate = orderDate;
        Subtotal = subtotal;
    }
    
}