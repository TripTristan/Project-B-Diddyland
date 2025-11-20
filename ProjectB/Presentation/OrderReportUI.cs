using System;
using System.Collections.Generic;

public static class OrderReportUI
{
    public static void Run()
    {
        Console.Clear();
        UiHelpers.WriteHeader("Order Reports (Super Admin)");

        int year = ReadInt("Year (e.g. 2025): ", y => y >= 2000 && y <= 2100);
        int month = ReadInt("Month (1-12, 0 = all months): ", m => m >= 0 && m <= 12);
        int day = ReadInt("Day (1-31, 0 = whole month): ", d => d >= 0 && d <= 31);

        var rows = ReportLogic.GetOrderReport(year, month, day);

        if (rows.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine("No bookings found for this period.");
            UiHelpers.Pause();
            return;
        }

        PrintSummary(rows);
        PrintTable(rows);

        UiHelpers.Pause();
    }

    private static int ReadInt(string prompt, Func<int, bool> isValid)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out int value) && isValid(value))
                return value;
            Console.WriteLine("Invalid value.");
        }
    }

    private static void PrintSummary(List<OrderReportRow> rows)
    {
        int totalBookings = rows.Count;
        int totalTickets = 0;
        decimal totalOriginal = 0m;
        decimal totalFinal = 0m;

        foreach (var r in rows)
        {
            totalTickets += r.Quantity;
            totalOriginal += r.OriginalPrice;
            totalFinal += r.FinalPrice;
        }

        Console.WriteLine();
        Console.WriteLine("Summary:");
        Console.WriteLine($"  Bookings : {totalBookings}");
        Console.WriteLine($"  Tickets  : {totalTickets}");
        Console.WriteLine($"  Original : {totalOriginal:C}");
        Console.WriteLine($"  Final    : {totalFinal:C}");
        Console.WriteLine();
    }

    private static void PrintTable(List<OrderReportRow> rows)
    {
        Console.WriteLine("Date       Time   OrderNumber                 CustId  SessId  Qty  Final");
        Console.WriteLine("----------------------------------------------------------------------------");
        foreach (var r in rows)
        {
            var dt = r.BookingDate;
            Console.WriteLine(
                $"{dt:yyyy-MM-dd} {dt:HH:mm}  {Trim(r.OrderNumber, 25),-25}  {r.CustomerId,6}  {r.SessionId,6}  {r.Quantity,3}  {r.FinalPrice,8:C}");
        }
    }

    private static string Trim(string value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty.PadRight(max);
        if (value.Length <= max) return value;
        return value.Substring(0, max - 1) + "…";
    }
}
