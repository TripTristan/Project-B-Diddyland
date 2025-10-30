public class CustmorOrderHistory
{
    private readonly CustmorOrderHistory  _logic;

    public CustmorOrderHistory(ReservationLogic logic)
    {
        _logic = logic;
    }

    public void DisplayOrderHistory()
    {
        Console.WriteLine("=== Order History ===");

        var orders = _logic.GetAllCustomerOrders();
        if (!orders.Any()) { Console.WriteLine("No orders found."); return; }

        var validOrders = orders.Select(o => new
        {
            Order = o,
            Date  = DateTime.TryParseExact(o.BookingDate, "yyyy-MM-dd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out var dt) ? dt : (DateTime?)null
        })
        .Where(x => x.Date.HasValue)
        .OrderBy(x => x.Date.Value);


        var yearGroups = validOrders.GroupBy(x => x.Date.Value.Year)
                                .OrderBy(g => g.Key);

        foreach (var yg in yearGroups)
        {
            Console.WriteLine($"\n【{yg.Key} Year】");
            foreach (var mg in yg.GroupBy(x => x.Date.Value.Month).OrderBy(g => g.Key))
            {
                var monthName = CultureInfo.GetCultureInfo("en-US")
                                        .DateTimeFormat.GetMonthName(mg.Key);

                Console.WriteLine($"  -- {monthName} --");
                foreach (var item in mg.OrderBy(x => x.Date.Value.Day))
                {
                    var o = item.Order;
                    Console.WriteLine($"Order Number: {o.OrderNumber}");
                    Console.WriteLine($"Session ID: {o.SessionId}");
                    Console.WriteLine($"Quantity: {o.Quantity}");
                    Console.WriteLine($"Booking Date: {o.BookingDate}");
                    Console.WriteLine($"Original Price: {o.OriginalPrice:C}");
                    Console.WriteLine($"Discount: {o.Discount:C}");
                    Console.WriteLine($"Final Price: {o.FinalPrice:C}");
                    Console.WriteLine("----------------------------------------------------------------\n\n\n");
                }
            }
        }

    }

}