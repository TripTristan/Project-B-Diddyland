using System;
using System.Collections.Generic;

public static class OrderReportUI
{
    public static void Run()
    {
        Console.Clear();
        UiHelpers.WriteHeader("Order Reports (Super Admin)");

        var today = DateTime.Today;

        Console.WriteLine("Select report type:");
        Console.WriteLine("1) Yearly report");
        Console.WriteLine("2) Monthly report");
        Console.WriteLine("3) Daily report");
        Console.WriteLine();

        int type;
        while (true)
        {
            Console.Write("Choice (1-3): ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out type) && type >= 1 && type <= 3)
                break;
            Console.WriteLine("Invalid value. Please enter 1, 2 or 3.");
        }

        int year;
        while (true)
        {
            Console.Write("Year (e.g. 2025): ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out year) && year >= 2000 && year <= today.Year)
                break;
            Console.WriteLine($"Invalid year. Please enter a value between 2000 and {today.Year}.");
        }

        int month = 0;
        int day = 0;

        if (type >= 2)
        {
            while (true)
            {
                int maxMonth = (year == today.Year) ? today.Month : 12;
                Console.Write($"Month (1-{maxMonth}): ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out month) && month >= 1 && month <= maxMonth)
                    break;
                Console.WriteLine($"Invalid month. Please enter a value between 1 and {maxMonth}.");
            }
        }

        if (type == 3)
        {
            while (true)
            {
                int maxDayInMonth = DateTime.DaysInMonth(year, month);
                int maxDay = (year == today.Year && month == today.Month)
                    ? Math.Min(maxDayInMonth, today.Day)
                    : maxDayInMonth;
                Console.Write($"Day (1-{maxDay}): ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out day) && day >= 1 && day <= maxDay)
                    break;
                Console.WriteLine("Invalid day. Please enter a valid day for the selected month.");
            }
        }

        var rows = ReportLogic.GetOrderReport(year, month, day);

        // Aarry {} //display // normale List
        var data = rows.ToArray();

        if (data.Length == 0)
        {
            Console.WriteLine();
            Console.WriteLine("No bookings found for this period.");
            UiHelpers.Pause();
            return;
        }

        PrintSummary(data);
        PrintTable(data);

        UiHelpers.Pause();
    }

    // Aarry {} //display // normale List
    private static void PrintSummary(OrderReportRow[] rows)
    {
        int totalBookings = rows.Length;
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

    private static void PrintTable(OrderReportRow[] rows)
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
