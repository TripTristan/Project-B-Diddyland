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

            Console.WriteLine("(Some bookings have no readable date; showing an ungrouped list.)\n");
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
        // Use OriginalPrice as the booking date (temporary fix)
        string formattedDate = FormatDateFromOriginalPrice(b.OriginalPrice);

        Console.WriteLine($"Order Number : {b.OrderNumber}");
        Console.WriteLine($"Quantity     : {b.Quantity}");
        Console.WriteLine($"Booking Date : {formattedDate}");
        Console.WriteLine($"Final        : {FormatCurrencyOrRaw(b.Discount)}");
        Console.WriteLine("------------------------------------------------\n");
    }

    private static string FormatDateFromOriginalPrice(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        // Remove any hyphen and whitespace
        raw = raw.Replace("-", "").Trim();

        // Expected pattern: ddMMyyyyHHmm or ddMMyyyy
        if (raw.Length >= 8)
        {
            try
            {
                string day = raw.Substring(0, 2);
                string month = raw.Substring(2, 2);
                string year = raw.Substring(4, 4);
                // Format as yyyy-MM-dd
                return $"{year}-{month}-{day}";
            }
            catch
            {
                // fallback: return as-is if substring fails
                return raw;
            }
        }

        return raw;
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
