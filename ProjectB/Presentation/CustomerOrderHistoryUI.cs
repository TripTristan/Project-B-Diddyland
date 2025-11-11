public class CustomerOrderHistoryUI
{
    private readonly ReservationLogic _logic;

    public CustomerOrderHistoryUI(ReservationLogic logic)
    {
        _logic = logic;
    }

    public void Display()
    {
        Console.WriteLine("=== Order History ===");

        var tree = _logic.GetCustomerOrdersGroupedByYearMonth(CustomerContext.CurrentId);
        if (!tree.Any()) // no orders
        {
            Console.WriteLine("No orders found.");
            return;
        }

        foreach (var yearGroup in tree)
        {
            Console.WriteLine($"\n【{yearGroup.Key} Year】");
            foreach (var monthGroup in yearGroup.OrderBy(mg => mg.Key))
            {
                var monthName = CultureInfo.GetCultureInfo("en-US")
                                          .DateTimeFormat.GetMonthName(monthGroup.Key);
                Console.WriteLine($"  -- {monthName} --");

                foreach (var order in monthGroup.OrderBy(o => o.BookingDate))
                {
                    Console.WriteLine($"Order Number: {o.OrderNumber}");
                    Console.WriteLine($"Session ID: {o.SessionId}");
                    Console.WriteLine($"Quantity: {o.Quantity}");
                    Console.WriteLine($"Booking Date: {o.BookingDate}");
                    Console.WriteLine($"Original Price: {o.OriginalPrice:C}");
                    Console.WriteLine($"Discount: {o.Discount:C}");
                    Console.WriteLine($"Final Price: {o.FinalPrice:C}");
                    Console.WriteLine("------------------------------------------------\n\n\n");
                }
                    

                }
            }
        }
    }
}