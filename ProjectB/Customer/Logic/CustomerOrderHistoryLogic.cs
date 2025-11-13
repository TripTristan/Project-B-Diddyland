internal static class Ext
{
    public static int ParseToInt(this string s) => int.Parse(s);
}

public class CustomerOrderHistoryLogic
{
    private readonly CustomerOrderHistoryAccess _repo;

    public CustomerOrderHistoryLogic(CustomerOrderHistoryAccess repo)
    {
        _repo = repo;
    }

    // // een interface waar foreach over kan lopen
    public IEnumerable<IGrouping<int, IGrouping<int, ReservationModel>>>
        GetCustomerOrdersGroupedByYearMonth(int customerId)
    {
        var valid = _repo.GetAllOrdersByCustomer(customerId)
                         .Select(o => new
                         {
                             Order = o,
                             Date = DateTime.TryParseExact(o.BookingDate,
                                                          "yyyy-MM-dd",
                                                          CultureInfo.InvariantCulture, // formatting independent // land 
                                                          DateTimeStyles.None,
                                                          out var dt) ? dt : (DateTime?)null
                         })
                         .Where(x => x.Date.HasValue)
                         .OrderBy(x => x.Date.Value) // sort by actual date bijv.: 2023-05-15 dan 2023-06-01
                         .Select(x => x.Order);

        return valid.GroupBy(o => o.BookingDate.Substring(0, 4).ParseToInt())
                   .Select(yg => new
                   {
                       Year = yg.Key,
                       Months = yg.GroupBy(o => o.BookingDate.Substring(5, 2).ParseToInt())
                   })
                   .OrderBy(x => x.Year)
                   .Select(x => x.Months.OrderBy(mg => mg.Key))
                   .SelectMany(x => x);
    }

}

