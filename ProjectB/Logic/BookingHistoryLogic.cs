using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class BookingHistoryLogic
{
    private static readonly Regex AnyTimestamp14 =
        new(@"(?<ts>\d{14})", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly string[] BookingDateFormats =
    {
        "yyyy-MM-dd",
        "yyyy-MM-dd HH:mm:ss",
        "ddMMyyyy-HHmm",
        "yyyyMMddHHmmss",
        "MM/dd/yyyy HH:mm",
        "MM/dd/yyyy",
        "o"
    };

    public static IEnumerable<IGrouping<int, IGrouping<int, BookingModel>>>
        GetUserBookingsGroupedByYearMonth(string username)
    {
        var rowsWithWhen = BookingAccess.GetByUsername(username)
            .Select(b => new { Booking = b, When = TryGetDate(b) })
            .OrderBy(x => x.When ?? DateTime.MaxValue)
            .ToList();

        var dated = rowsWithWhen.Where(x => x.When.HasValue)
                                .OrderBy(x => x.When!.Value)
                                .Select(x => x.Booking)
                                .ToList();

        return dated
            .GroupBy(b => EnsureDate(b).Year)
            .Select(yg => new { Year = yg.Key, Months = yg.GroupBy(b => EnsureDate(b).Month) })
            .OrderBy(x => x.Year)
            .Select(x => x.Months.OrderBy(mg => mg.Key))
            as IEnumerable<IGrouping<int, IGrouping<int, BookingModel>>>;
    }

    public static List<BookingModel> GetUserBookingsRaw(string username)
        => BookingAccess.GetByUsername(username).ToList();

    private static readonly Dictionary<string, DateTime> Cache = new();

    private static DateTime EnsureDate(BookingModel b)
    {
        if (b == null || string.IsNullOrEmpty(b.OrderNumber))
            return DateTime.MinValue;

        if (Cache.TryGetValue(b.OrderNumber, out var dt))
            return dt;

        var parsed = TryGetDate(b) ?? DateTime.MinValue;
        Cache[b.OrderNumber] = parsed;
        return parsed;
    }

    private static DateTime? TryGetDate(BookingModel b)
    {
        if (!string.IsNullOrWhiteSpace(b.BookingDate))
        {
            if (DateTime.TryParseExact(b.BookingDate, BookingDateFormats,
                                       CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromField))
                return fromField;

            if (DateTime.TryParse(b.BookingDate, out var loose))
                return loose;
        }


        if (!string.IsNullOrWhiteSpace(b.OrderNumber))
        {
            var m = AnyTimestamp14.Match(b.OrderNumber);
            if (m.Success &&
                DateTime.TryParseExact(m.Groups["ts"].Value, "yyyyMMddHHmmss",
                                       CultureInfo.InvariantCulture, DateTimeStyles.None,
                                       out var fromOrd))
                return fromOrd;
        }

        if (!string.IsNullOrWhiteSpace(b.OriginalPrice))
        {
            if (DateTime.TryParseExact(b.OriginalPrice, BookingDateFormats,
                                       CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromPrice))
                return fromPrice;
        }

        return null;
    }
}
