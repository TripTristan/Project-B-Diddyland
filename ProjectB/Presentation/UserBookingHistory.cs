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
    private string FormatSession(SessionModel session)
    {
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

            return sessionDateFormatted = $"{sessionDate.Split(' ')[0]} {timeSlot}";
        }
    }
    private void PrintBooking(BookingModel b)
    {
        string bookingDateFormatted = FormatTicksOrDate(b.BookingDate);
        string typeText = ConvertType(b.Type);

        var session = _sessionAccess.GetSessionById(b.SessionId);
        string sessionDateFormatted = "Unknown session";

        if (session != null)
        {
            sessionDateFormatted = FormatSession(session);
        }

        Console.WriteLine($"\n------------------------------------------------\nOrder Number : {b.OrderNumber}\nType         : {typeText}\nQuantity     : {b.Quantity}\nBooked On    : {bookingDateFormatted}\nSession Time : {sessionDateFormatted}\nFinal Price  : {b.Price:C}\n------------------------------------------------\n");
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
        if (DateTime.TryParse(raw, out var dt))
            return dt.ToString("dd-MM-yyyy HH:mm");

        if (long.TryParse(raw, out var ticks))
        {
            return new DateTime(ticks).ToString("dd-MM-yyyy HH:mm");
        }

        return raw;

        /*
        PREVIOUS CODE INCASE STUFF FAILS
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
        */
    }
}
