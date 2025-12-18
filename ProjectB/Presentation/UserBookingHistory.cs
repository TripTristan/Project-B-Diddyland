using System;
using System.Globalization;
using System.Linq;

public class BookingHistory
{
    private readonly BookingHistoryLogic _logic;
    private readonly SessionAccess _sessionAccess;
    private readonly AttractiesAccess _attractiesAccess;

    public BookingHistory(
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
        Console.WriteLine("=== My Bookings ===");

        Console.WriteLine("\nFilter your bookings:");
        Console.WriteLine("  1) Show ALL bookings");
        Console.WriteLine("  2) Only Reservations");
        Console.WriteLine("  3) Only FastPass");
        Console.Write("\nChoose an option: ");

        string? choice = Console.ReadLine()?.Trim();

        Func<BookingModel, bool> filter = choice switch
        {
            "2" => b => b.Type == "0", // Reservation
            "3" => b => b.Type == "1", // FastPass
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
            return;
        }

        foreach (var b in bookings)
            PrintBooking(b);
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
                timeIndex >= 0 && timeIndex < UserReservation.TimeslotOptions.Count
                    ? UserReservation.TimeslotOptions[timeIndex]
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
