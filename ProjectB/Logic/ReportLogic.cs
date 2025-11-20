using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class ReportLogic
{
    public static List<OrderReportRow> GetOrderReport(int year, int month, int day)
    {
        var bookings = GetAllBookingsFromDatabase();

        var filtered = bookings.Where(b =>
        {
            if (!TryParseBookingDate(b.BookingDate, out var dt))
                return false;

            if (dt.Year != year)
                return false;

            if (month > 0 && dt.Month != month)
                return false;

            if (day > 0 && dt.Day != day)
                return false;

            return true;
        });

        var rows = filtered
            .Select(b => new OrderReportRow
            {
                OrderNumber = b.OrderNumber,
                BookingDate = ParseBookingDateOrDefault(b.BookingDate),
                CustomerId = b.CustomerID,
                SessionId = b.SessionId,
                Quantity = b.Quantity,
                OriginalPrice = b.OriginalPrice,
                Discount = b.Discount,
                FinalPrice = b.FinalPrice
            })
            .OrderBy(r => r.BookingDate)
            .ThenBy(r => r.OrderNumber)
            .ToList();

        return rows;
    }

    private static bool TryParseBookingDate(string value, out DateTime date)
    {
        return DateTime.TryParseExact(
            value,
            "ddMMyyyy-HHmm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    private static DateTime ParseBookingDateOrDefault(string value)
    {
        return TryParseBookingDate(value, out var dt) ? dt : DateTime.MinValue;
    }

    private static List<ReservationModel> GetAllBookingsFromDatabase()
    {
        return ReservationAccess.GetAllBookings();
    }
}
