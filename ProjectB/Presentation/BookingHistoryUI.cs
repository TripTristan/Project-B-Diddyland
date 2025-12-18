using System;
using System.Globalization;
using System.Linq;

public class BookingHistoryUI
{
    private readonly BookingHistoryLogic _logic;
    private readonly SessionAccess _sessionAccess;
    private readonly AttractiesAccess _attractiesAccess;

    public BookingHistoryUI(
        BookingHistoryLogic logic,
        SessionAccess sessionAccess,
        AttractiesAccess attractiesAccess)
    {
        _logic = logic;
        _sessionAccess = sessionAccess;
        _attractiesAccess = attractiesAccess;
    }

    public void Display(string username)
    {
        List<List<string>> filterOptions = new List<List<string>>
        {
            new List<string> { "Show ALL bookings" },
            new List<string> { "Only Reservations" },
            new List<string> { "Only FastPass" },
            new List<string> { "Go Back" }
        };

        MainMenu filterMenu = new MainMenu(filterOptions, "Filter your bookings:");
        int[] result = filterMenu.Run();

        if (result[0] == 3)
            return;

        Func<BookingModel, bool> filter = result[0] switch
        {
            1 => b => b.Type == "0",
            2 => b => b.Type == "1",
            _   => b => true
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

    private void PrintBooking(BookingModel b)
    {
        string bookingDateFormatted = FormatTicksOrDate(b.BookingDate);
        string typeText = ConvertType(b.Type);

        var session = _sessionAccess.GetSessionById(b.SessionId);
        string sessionDateFormatted = "Unknown session";

        if (session != null)
        {
            string sessionDate = FormatTicksOrDate(session.Date.ToString());

            int timeIndex =
                session.Time >= int.MinValue && session.Time <= int.MaxValue
                    ? (int)session.Time
                    : -1;

            string timeSlot =
                timeIndex >= 0 && timeIndex < ReservationUI.TimeslotOptions.Count
                    ? ReservationUI.TimeslotOptions[timeIndex]
                    : "Unknown time";

            sessionDateFormatted = $"{sessionDate.Split(' ')[0]} {timeSlot}";
        }

        Console.WriteLine("\n------------------------------------------------");
        Console.WriteLine($"Order Number : {b.OrderNumber}");
        Console.WriteLine($"Type         : {typeText}");
        Console.WriteLine($"Quantity     : {b.Quantity}");
        Console.WriteLine($"Booked On    : {bookingDateFormatted}");
        Console.WriteLine($"Session Time : {sessionDateFormatted}");
        Console.WriteLine($"Final Price  : {b.Price:C}");
        Console.WriteLine("------------------------------------------------\n");
    }

    // =========================
    // Helpers
    // =========================

    private static string ConvertType(string rawType)
    {
        return rawType switch
        {
            "0" => "Reservation",
            "1" => "FastPass",
            _   => rawType
        };
    }

    private static string FormatTicksOrDate(string raw)
    {
        if (long.TryParse(raw, out var ticks))
        {
            try
            {
                return new DateTime(ticks).ToString("dd-MM-yyyy HH:mm");
            }
            catch
            {
                return raw;
            }
        }

        if (DateTime.TryParse(raw, out var dt))
            return dt.ToString("dd-MM-yyyy HH:mm");

        return raw;
    }
}
