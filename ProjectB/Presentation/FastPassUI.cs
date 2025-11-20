using System;
using System.Linq;
using System.Globalization;

public static class FastPassUI
{
    public static void Run(UserModel? currentUser = null)
    {
        Console.Clear();
        Console.WriteLine("=== FastPass Reservation ===\n");

        string location = ChooseParkLocation();

        var attractions = AttractiesAccess.GetAll()
                            .Where(a => a.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                            .ToList();

        if (attractions.Count == 0)
        {
            Console.WriteLine($"No attractions found in {location}. Please add attractions first.");
            return;
        }

        Console.WriteLine($"\nSelect an attraction in {location}:");
        foreach (var a in attractions)
            Console.WriteLine($"  {a.ID}. {a.Name} (Type: {a.Type}, MinHeight: {a.MinHeightInCM}cm, MaxCapacity: {a.Capacity})");

        Console.WriteLine("\nEnter attraction ID (0 to cancel): ");
        int attractionId = ReadInt("> ",
            id => attractions.Any(a => a.ID == id),
            "Invalid attraction ID.",
            allowCancel: true);

        if (attractionId == 0)
        {
            Console.WriteLine("FastPass cancelled.");
            return;
        }

        DateTime day = DateTime.Today;

        var available = FastPassLogic.GetAvailableFastPassSessions(attractionId, day, location);
        if (available.Count == 0)
        {
            Console.WriteLine($"\nNo available timeslots for this attraction today in {location}.");
            return;
        }

        Console.WriteLine($"\nAvailable timeslots for {day:yyyy-MM-dd}:");
        for (int i = 0; i < available.Count; i++)
        {
            var s = available[i];
            int cap = SessionAccess.GetCapacityBySession(s);
            Console.WriteLine($"  [{i + 1}] {s.Time}  (Booked: {s.CurrentBookings}/{cap})");
        }

        Console.WriteLine("\nChoose a timeslot (0 to cancel): ");
        int index = ReadInt("> ",
            n => n >= 1 && n <= available.Count,
            "Please choose a valid timeslot number.",
            allowCancel: true);

        if (index == 0)
        {
            Console.WriteLine("FastPass cancelled.");
            return;
        }

        var selectedSession = available[index - 1];

        int qty = ReadInt("How many tickets?: ", n => n > 0, "Quantity must be a positive number.");

        try
        {
            var confirmation = FastPassLogic.BookFastPass(selectedSession.Id, qty, currentUser, location);

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

    private static string ChooseParkLocation()
    {
        string[] locations = { "DiddyLand - Amsterdam", "DiddyLand - Rotterdam" };

        while (true)
        {
            Console.WriteLine("Select park location:");
            for (int i = 0; i < locations.Length; i++)
                Console.WriteLine($"{i + 1}) {locations[i]}");

            Console.Write("\nEnter choice number: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= locations.Length)
                return locations[choice - 1];

            Console.WriteLine("Invalid input. Please choose a valid number.\n");
        }
    }

    private static int ReadInt(string prompt, Func<int, bool> isValid, string errorMsg, bool allowCancel = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (input == "0" && allowCancel)
                return 0;

            if (int.TryParse(input, out int val) && isValid(val))
                return val;

            Console.WriteLine(errorMsg);
        }
    }
}
