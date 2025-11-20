using System;
using System.Globalization;
using System.Linq;

public static class BookingHistoryUI
{
    public static void Display(string username)
    {
        Console.WriteLine("=== My Bookings ===");

        var tree = BookingHistoryLogic.GetUserBookingsGroupedByYearMonth(username);

        if (tree == null || !tree.Any())
        {
            var raw = BookingHistoryLogic.GetUserBookingsRaw(username);
            if (raw == null || raw.Count == 0)
            {
                Console.WriteLine("No bookings found.");
                return;
            }

            foreach (var b in raw.OrderBy(b => b.OrderNumber))
            {
                PrintBooking(b);
            }
            return;
        }

        foreach (var yearGroup in tree)
        {
            Console.WriteLine($"\n【{yearGroup.Key} Year】");
            foreach (var monthGroup in yearGroup.OrderBy(mg => mg.Key))
            {
                var monthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(monthGroup.Key);
                Console.WriteLine($"  -- {monthName} --");

                foreach (var b in monthGroup.OrderBy(bm => bm.OrderNumber))
                {
                    PrintBooking(b);
                }
            }
        }
    }

    private static void PrintBooking(BookingModel b)
    {
        string bookingDateFormatted = "";

        if (DateTime.TryParse(b.BookingDate, out var bookingDt))
            bookingDateFormatted = bookingDt.ToString("dd-MM-yyyy HH:mm");
        else
            bookingDateFormatted = b.BookingDate;


        var session = SessionAccess.GetSessionById(b.SessionId);

        string sessionDateFormatted = "Unknown session";

        if (session != null)
        {
            string combined = $"{session.Date} {session.Time}";

            if (DateTime.TryParseExact(
                    session.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var sessionDate))
            {
                sessionDateFormatted = sessionDate.ToString("dd-MM-yyyy") + " " + session.Time;
            }
            else
            {
                sessionDateFormatted = session.Date + " " + session.Time;
            }
        }

        Console.WriteLine($"Order Number : {b.OrderNumber}");
        Console.WriteLine($"Quantity     : {b.Quantity}");
        Console.WriteLine($"Booked On    : {bookingDateFormatted}");
        Console.WriteLine($"Session Time : {sessionDateFormatted}");
        Console.WriteLine($"Final Price  : {b.FinalPrice:C}");
        Console.WriteLine("------------------------------------------------\n");
    }


    private static string FormatCurrencyOrRaw(string value)
    {
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
            return dec.ToString("C", CultureInfo.CurrentCulture);
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out dec))
            return dec.ToString("C", CultureInfo.CurrentCulture);
        return value ?? "";
    }
}
