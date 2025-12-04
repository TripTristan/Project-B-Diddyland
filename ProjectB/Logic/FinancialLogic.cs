static class FinancialLogic
{
    public static DateTime GetDateFromCoordinate(int[] coord, int year, int month)
    {
        int row = coord[0];
        int col = coord[1];
        int day = row * 7 + col + 1;
        int daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth) day = daysInMonth;
        return new DateTime(year, month, day);
    }

    public static List<ReservationModel> GetRevenueByDateRange(long beginDate, long endDate)
    {
        List<ReservationModel> Orders = ReservationAccess.GetAllOrdersBetweenDates(beginDate, endDate);
        return Orders;
    }

    public static List<ReservationModel> GetAllUserOrders(UserModel selectedUser)
    {
        List<ReservationModel> orders = ReservationAccess.GetAllBookingsByUserID(selectedUser.Id);
        return orders;
    }

    public static void GraphByDateAndRevenue(List<long> date, List<double> revenue)
    {
        Console.WriteLine();
    }

    
}
