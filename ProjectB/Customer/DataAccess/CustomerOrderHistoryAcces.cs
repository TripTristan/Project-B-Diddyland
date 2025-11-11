public class CustomerOrderHistoryAcces
{
    public IEnumerable<ReservationModel> GetAllOrdersByCustomer(int customerId)
    {
        return _allOrders.Where(o => o.CustomerId == customerId).ToList();
    }
}