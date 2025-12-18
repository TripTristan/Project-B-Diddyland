using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

public class FastPassUI
{
    private readonly FastPassLogic _fastPassLogic;
    private readonly AttractiesAccess _attractiesAccess;
    private readonly SessionAccess _sessionAccess;
    private readonly UiHelpers _ui;
    private readonly DiscountCodeLogic _discountLogic;

    public FastPassUI(
        FastPassLogic fastPassLogic,
        AttractiesAccess attractiesAccess,
        SessionAccess sessionAccess,
        UiHelpers ui,
        DiscountCodeLogic discountLogic)
    {
        _fastPassLogic = fastPassLogic;
        _attractiesAccess = attractiesAccess;
        _sessionAccess = sessionAccess;
        _ui = ui;
        _discountLogic = discountLogic;
    }

    public void Run(UserModel? currentUser = null)
    {
        Console.Clear();
        Console.WriteLine("=== FastPass Reservation ===\n");

        string? location = ChooseParkLocation();
        if (location == null)
            return;

        var attractions = _attractiesAccess.GetAll()
            .Where(a => a.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (attractions.Count == 0)
        {
            Console.WriteLine($"No attractions found in {location}. Please add attractions first.");
            return;
        }

        List<List<string>> attractionOptions = attractions
            .Select(a => new List<string> { $"{a.Name} (Type: {a.Type}, MinHeight: {a.MinHeightInCM}cm)" })
            .ToList();
        attractionOptions.Add(new List<string> { "Cancel" });

        MainMenu attractionMenu = new MainMenu(attractionOptions, $"Select an attraction in {location}:");
        int[] attractionResult = attractionMenu.Run();

        if (attractionResult[0] == attractions.Count)
        {
            Console.WriteLine("FastPass cancelled.");
            return;
        }

        int attractionId = attractions[attractionResult[0]].ID;
        DateTime day = DateTime.Today;

        var available = _fastPassLogic.GetAvailableFastPassSessions(attractionId, day, location);
        if (available.Count == 0)
        {
            Console.WriteLine($"\nNo available timeslots for this attraction today in {location}.");
            UiHelpers.Pause();
            return;
        }

        List<List<string>> timeslotOptions = available
            .Select(s => 
            {
                string timeDisplay = (int)s.Time > 0 && (int)s.Time <= ReservationUI.TimeslotOptions.Count
                    ? ReservationUI.TimeslotOptions[(int)s.Time - 1]
                    : $"Slot {s.Time}";
                return new List<string> { $"{timeDisplay} (Available: {s.Capacity}/35)" };
            })
            .ToList();
        timeslotOptions.Add(new List<string> { "Cancel" });

        MainMenu timeslotMenu = new MainMenu(timeslotOptions, $"Available timeslots for {day:yyyy-MM-dd}:");
        int[] timeslotResult = timeslotMenu.Run();

        if (timeslotResult[0] == available.Count)
        {
            Console.WriteLine("FastPass cancelled.");
            return;
        }

        var selectedSession = available[timeslotResult[0]];

        int qty = ReadInt("How many tickets?: ",
            n => n > 0,
            "Quantity must be a positive number.");

        const double basePrice = 10.0;
        double originalTotal = qty * basePrice;

        Console.Write("\nDo you have a discount code? (enter or leave blank): ");
        string? code = Console.ReadLine()?.Trim();
        
        double finalTotal = _discountLogic.Apply(code, originalTotal);

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
            UiHelpers.Pause();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nBooking failed: {ex.Message}");
            UiHelpers.Pause();
        }
    }

    private string? ChooseParkLocation()
    {
        List<List<string>> locationOptions = new List<List<string>>
        {
            new List<string> { "DiddyLand - Amsterdam" },
            new List<string> { "DiddyLand - Rotterdam" },
            new List<string> { "Go Back" }
        };

        MainMenu locationMenu = new MainMenu(locationOptions, "Select park location:");
        int[] result = locationMenu.Run();

        if (result[0] == 2)
            return null;

        return locationOptions[result[0]][0];
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
