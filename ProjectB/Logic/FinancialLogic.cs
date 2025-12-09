public class FinancialLogic
{
    private readonly ReservationAccess _reservationAccess;
    private readonly UserAccess _userAccess;

    public FinancialLogic(ReservationAccess reservationAccess, UserAccess userAccess)
    {
        _reservationAccess = reservationAccess;
        _userAccess = userAccess;
    }

    public DateTime GetDateFromCoordinate(int[] coord, int year, int month)
    {
        int row = coord[0];
        int col = coord[1];
        int day = row * 7 + col + 1;
        int daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth) day = daysInMonth;
        return new DateTime(year, month, day);
    }

    public List<ReservationModel> GetRevenueByDateRange(long beginDate, long endDate)
    {
        List<ReservationModel> Orders = _reservationAccess.GetAllOrdersBetweenDates(beginDate, endDate);
        return Orders;
    }

    public List<ReservationModel> GetAllUserOrders(UserModel selectedUser)
    {
        List<ReservationModel> orders = _reservationAccess.GetAllBookingsByUserID(selectedUser.Id);
        return orders;
    }

    public void GraphByDateAndRevenue(List<long> date, List<double> revenue)
    {
        Console.WriteLine();
    }

    public List<UserModel> GrabAllUsers()
    {
        return _userAccess.GetAllUsers().ToList();
    }


    
}
