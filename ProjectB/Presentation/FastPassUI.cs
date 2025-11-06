using System;
using System.Linq;
using System.Globalization;

public static class FastPassUI
{
    public static void Run(UserModel? currentUser = null)
    {
        Console.Clear();
        Console.WriteLine("=== FastPass Reservation ===\n");

        var attractions = AttractiesAccess.GetAll().ToList();
        if (attractions.Count == 0)
        {
            Console.WriteLine("No attractions found. Please add attractions first.");
            return;
        }

        Console.WriteLine("Select an attraction:");
        foreach (var a in attractions)
            Console.WriteLine($"  {a.ID}. {a.Name} (Type: {a.Type}, MinHeight: {a.MinHeightInCM}cm, Capacity: {a.Capacity})");

        int attractionId = ReadInt("\nEnter attraction ID: ", id => attractions.Any(a => a.ID == id), "Invalid attraction ID.");

        DateTime day = DateTime.Today;

        var available = FastPassLogic.GetAvailableFastPassSessions(attractionId, day);
        if (available.Count == 0)
        {
            Console.WriteLine("\nNo available timeslots for this attraction today.");
            return;
        }

        Console.WriteLine($"\nAvailable timeslots for {day:yyyy-MM-dd}:");
        for (int i = 0; i < available.Count; i++)
        {
            var s = available[i];
            int cap = SessionAccess.GetCapacityBySession(s);
            Console.WriteLine($"  [{i + 1}] {s.Time}  (Booked: {s.CurrentBookings}/{cap})");
        }

        int index = ReadInt("\nChoose a timeslot (number): ",
                            n => n >= 1 && n <= available.Count,
                            "Please choose a valid timeslot number.");
        var selectedSession = available[index - 1];

        int qty = ReadInt("How many tickets?: ", n => n > 0, "Quantity must be a positive number.");

        try
        {
            var confirmation = FastPassLogic.BookFastPass(selectedSession.Id, qty, currentUser);

            Console.Clear();
            Console.WriteLine("====== FastPass Confirmation ======\n");
            Console.WriteLine($"Order Number : {confirmation.OrderNumber}");
            Console.WriteLine($"Type         : {confirmation.Type}");
            Console.WriteLine($"Attraction   : {confirmation.Attraction}");
            Console.WriteLine($"Date         : {confirmation.Date}");
            Console.WriteLine($"Time         : {confirmation.Time}");
            Console.WriteLine($"Quantity     : {confirmation.Quantity}");
            Console.WriteLine($"Price/person : {confirmation.PricePerPerson:C}");
            Console.WriteLine($"Total Price  : {confirmation.TotalPrice:C}");
            Console.WriteLine("\nThank you and enjoy your ride!");
            Console.WriteLine("===================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nBooking failed: {ex.Message}");
        }
    }

    private static int ReadInt(string prompt, Func<int, bool> isValid, string errorMsg)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out int val) && isValid(val))
                return val;
            Console.WriteLine(errorMsg);
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        Console.Write(prompt);
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return DateTime.Today;
        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;
        Console.WriteLine("Invalid date. Using today.");
        return DateTime.Today;
    }
}

