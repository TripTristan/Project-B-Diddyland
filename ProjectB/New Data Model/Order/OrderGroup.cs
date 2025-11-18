public class OrderGroup : OrderBase
{
    GroupType GroupType { get; set; }
    public int GroupSize { get; set; }
    public decimal GroupDiscount { get; set; }
    
    public OrderGroup(
        string orderNumber,
        int customerId,
        DateTime orderDate,
        decimal subtotal,
        GroupType groupType,
        int groupSize,
        decimal groupDiscount) : base ( orderNumber,
                                        customerId,
                                        orderDate,
                                        subtotal){
        GroupType = groupType;
        GroupSize = groupSize;
        GroupDiscount = groupDiscount;
    }
    
}