public class OrderBase
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string OrderNumber { get; set; } = "";
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Subtotal { get; set; }
    
    public OrderBase(
        string orderNumber,
        int customerId,
        DateTime orderDate,
        decimal subtotal)
    {
        OrderNumber = orderNumber;
        CustomerId = customerId;
        OrderDate = orderDate;
        Subtotal = subtotal;
    }
    
}