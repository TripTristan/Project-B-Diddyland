using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

public class BookingHistoryLogic
{
    private readonly IBookingAccess _bookingAccess;
    private readonly SessionAccess _sessionAccess;

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

    private readonly Dictionary<string, DateTime> _cache = new();

    public BookingHistoryLogic(Dependencies dp)
    {
        _bookingAccess = dp.bookingAccess;
        _sessionAccess = dp.sessionAccess;
    }

    public SessionModel RetrieveSession(int id)
    {
        return _sessionAccess.GetSessionById(id);
    }
    public IEnumerable<IGrouping<int, IGrouping<int, BookingModel>>>
        GetUserBookingsGroupedByYearMonth(string username)
    {
        var rowsWithWhen = _bookingAccess.GetByUsername(username)
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

    public List<BookingModel> GetUserBookingsRaw(string username)
        => _bookingAccess.GetByUsername(username).ToList();

    private DateTime EnsureDate(BookingModel b)
    {
        if (b == null || string.IsNullOrEmpty(b.OrderNumber))
            return DateTime.MinValue;

        if (_cache.TryGetValue(b.OrderNumber, out var dt))
            return dt;

        var parsed = TryGetDate(b) ?? DateTime.MinValue;
        _cache[b.OrderNumber] = parsed;
        return parsed;
    }

    private DateTime? TryGetDate(BookingModel b)
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

        return null;
    }
}
