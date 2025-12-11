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
        Console.WriteLine("=== My Bookings ===");

        Console.WriteLine("\nFilter your bookings:");
        Console.WriteLine("  1) Show ALL bookings");
        Console.WriteLine("  2) Only Reservations");
        Console.WriteLine("  3) Only FastPass");
        Console.Write("\nChoose an option: ");

        string? choice = Console.ReadLine()?.Trim();

        Func<BookingModel, bool> filter = choice switch
        {
            "2" => b => b.Type == "Reservation",
            "3" => b => b.Type == "FastPass",
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
        string bookingDateFormatted =
            DateTime.TryParse(b.BookingDate, out var bookingDt)
            ? bookingDt.ToString("dd-MM-yyyy HH:mm")
            : b.BookingDate;

        var session = _sessionAccess.GetSessionById(b.SessionId);

        string sessionDateFormatted = "Unknown session";

        if (session != null)
        {
            if (DateTime.TryParseExact(
                    session.Date.ToString("yyyy-MM-dd"),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var sessionDate))
            {
                sessionDateFormatted = $"{sessionDate:dd-MM-yyyy} {session.Time}";
            }
            else
            {
                sessionDateFormatted = $"{session.Date} {session.Time}";
            }
        }

        Console.WriteLine("\n------------------------------------------------");
        Console.WriteLine($"Order Number : {b.OrderNumber}");
        Console.WriteLine($"Type         : {b.Type}");
        Console.WriteLine($"Quantity     : {b.Quantity}");
        Console.WriteLine($"Booked On    : {bookingDateFormatted}");

        if (b.Type == "FastPass" && session != null)
        {
            // var attraction = _attractiesAccess.GetById(session);
            // string name = attraction?.Name ?? $"Attraction #{session}";
            // Console.WriteLine($"Attraction   : {name}");
            Console.WriteLine();
        }

        Console.WriteLine($"Session Time : {sessionDateFormatted}");
        Console.WriteLine($"Final Price  : {b.Price/100:C}");
        Console.WriteLine("------------------------------------------------\n");
    }
    
    private string FormatCurrencyOrRaw(string value)
    {
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
            return dec.ToString("C", CultureInfo.CurrentCulture);

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out dec))
            return dec.ToString("C", CultureInfo.CurrentCulture);

        return value ?? "";
    }
}
