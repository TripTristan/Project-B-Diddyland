using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

public class FastPass
{
    private readonly UserContext _context;

    public FastPass(UserContext context)
    {
        _context = context;
    }
    public void Run(UserModel? currentUser = null)
    {
        Console.Clear();
        Console.WriteLine("=== FastPass Reservation ===\n");

        string location = ChooseParkLocation();

        var attractions = _attractiesAccess.GetAll()
            .Where(a => a.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (attractions.Count == 0)
        {
            Console.WriteLine($"No attractions found in {location}. Please add attractions first.");
            return;
        }

        Console.WriteLine($"\nSelect an attraction in {location}:");
        foreach (var a in attractions)
        {
            Console.WriteLine(
                $"  {a.ID}. {a.Name} (Type: {a.Type}, MinHeight: {a.MinHeightInCM}cm, MaxCapacity: {a.Capacity})"
            );
        }

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

        var available = _fastPassLogic.GetAvailableFastPassSessions(attractionId, day, location);
        if (available.Count == 0)
        {
            Console.WriteLine($"\nNo available timeslots for this attraction today in {location}.");
            return;
        }

        Console.WriteLine($"\nAvailable timeslots for {day:yyyy-MM-dd}:");

        for (int i = 0; i < available.Count; i++)
        {
            var s = available[i];
            long cap = s.Capacity;

            Console.WriteLine($"  [{i + 1}] {s.Time} (Booked: {s.Capacity}/{cap})");
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

        int qty = ReadInt("How many tickets?: ",
            n => n > 0,
            "Quantity must be a positive number.");

        const double basePrice = 10.0;
        double originalTotal = qty * basePrice;

        Console.Write("\nDo you have a discount code? (enter or leave blank): ");
        string? code = Console.ReadLine()?.Trim();

        double finalTotal = _ctx.discountLogic.Apply(code, originalTotal);

        Console.WriteLine($"\nFinal Price (after discount if any): {finalTotal:C}\n");

        try
        {
            var confirmation = _fastPassLogic.BookFastPass(selectedSession.Id, qty, currentUser, location);

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

    private string ChooseParkLocation()
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

    private int ReadInt(string prompt, Func<int, bool> isValid, string errorMsg, bool allowCancel = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (allowCancel && input == "0")
                return 0;

            if (int.TryParse(input, out int val) && isValid(val))
                return val;

            Console.WriteLine(errorMsg);
        }
    }

    private DateTime ReadDate(string prompt)
    {
        Console.Write(prompt);
        var s = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(s))
            return DateTime.Today;

        if (DateTime.TryParseExact(s, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;

        Console.WriteLine("Invalid date. Using today.");
        return DateTime.Today;
    }
}
