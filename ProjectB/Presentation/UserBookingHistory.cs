using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

public class BookingHistory
{
    private readonly BookingHistoryLogic _logic;

    public BookingHistory(BookingHistoryLogic logic)
    {
        _logic = logic;
    }

    public void Display(string username)
    {
        Console.WriteLine("=== My Bookings ===");

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Show ALL bookings"},
            new List<string> {"Only Reservations"},
            new List<string> {"Only FastPass"}
        };

        MainMenu Menu = new MainMenu(Options, "Filter your bookings:");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        Func<BookingModel, bool> filter = selectedIndex[0] switch
        {
            1 => b => b.Type == "0", // Reservation
            2 => b => b.Type == "1", // FastPass
            _ => b => true
        };

        var bookings = _logic
            .GetUserBookingsRaw(username)
            .Where(filter)
            .OrderBy(b => b.OrderNumber)
            .ToList();

        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found for this filter.");
            UiHelpers.Pause();
            return;
        }

        foreach (var b in bookings)
            PrintBooking(b);

        UiHelpers.Pause();
    }

    private string FormatSession(SessionModel session)
    {
        string sessionDate = FormatTicksOrDate(session.Date.ToString());
        string dateOnly = sessionDate.Split(' ')[0];

        string timeSlot = FormatSessionTime(session.Time);

        return $"{dateOnly} {timeSlot}";
    }

    // ✅ NEW: handles BOTH normal (1..3) AND fastpass (ticks)
    private static string FormatSessionTime(long time)
    {
        // Normal sessions are stored as 1..3
        if (time == 1) return UserReservation.TimeslotOptions[0];
        if (time == 2) return UserReservation.TimeslotOptions[1];
        if (time == 3) return UserReservation.TimeslotOptions[2];

        // FastPass sessions are stored as ticks-from-midnight
        try
        {
            var ts = new TimeSpan(time);
            if (ts >= TimeSpan.Zero && ts < TimeSpan.FromDays(1))
                return ts.ToString(@"hh\:mm");
        }
        catch { }

        return "Unknown time";
    }

    private void PrintBooking(BookingModel b)
    {
        string bookingDateFormatted = FormatTicksOrDate(b.BookingDate);
        string typeText = ConvertType(b.Type);

        var session = _logic.RetrieveSession(b.SessionId);
        string sessionDateFormatted = "Unknown session";

        if (session != null)
            sessionDateFormatted = FormatSession(session);

        Console.WriteLine(
            $"\n------------------------------------------------\n" +
            $"Order Number : {b.OrderNumber}\n" +
            $"Type         : {typeText}\n" +
            $"Quantity     : {b.Quantity}\n" +
            $"Booked On    : {bookingDateFormatted}\n" +
            $"Session Time : {sessionDateFormatted}\n" +
            $"Final Price  : {b.Price:C}\n" +
            $"------------------------------------------------\n");
    }

    private static string ConvertType(string rawType)
    {
        return rawType switch
        {
            "0" => "Reservation",
            "1" => "FastPass",
            _ => rawType
        };
    }

    private static string FormatTicksOrDate(string raw)
    {
        if (DateTime.TryParse(raw, out var dt))
            return dt.ToString("dd-MM-yyyy HH:mm");

        if (long.TryParse(raw, out var ticks))
            return new DateTime(ticks).ToString("dd-MM-yyyy HH:mm");

        return raw;
    }
}
