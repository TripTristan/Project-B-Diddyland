public class OrderBase
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string Nr { get; set; } = "";
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Subtotal { get; set; }
    
    public OrderBase(
        string nr,
        int customerId,
        DateTime orderDate,
        decimal subtotal)
    {
        Nr = nr;
        CustomerId = customerId;
        OrderDate = orderDate;
        Subtotal = subtotal;
    }
    
}