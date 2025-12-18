using System;
using System.Collections.Generic;
using System.Linq;

public enum ReservationType
{
    Normal,
    Group
}

public class UserReservation
{
    private UserContext _ctx;
    public UserReservation(UserContext a) { _ctx = a; }

    public static List<string> AgeOptions = new() { "0-15   : ", "16-60 : ", "61+   : " };
    public static List<string> TimeslotOptions = new() { "09:00-13:00", "13:00-17:00", "17:00-21:00" };

    public void Book()
    {
        ReservationType reservationType = SelectReservationType();

        DateTime chosenDate = DatePicker();
        SessionModel session = ShowTimeslotsByDate(
            _ctx.reservationLogic.GetSessionsByDate(chosenDate));

        List<int> guests = GuestQuantitySelection();
        int totalGuests = guests.Sum();

        _ctx.reservationLogic.ValidateReservationType(totalGuests, reservationType);

        double basePrice = _ctx.reservationLogic.CalculatePriceForGuests(guests);
        double finalPrice;

        if (reservationType == ReservationType.Group)
        {
            finalPrice = _ctx.reservationLogic.ApplyGroupDiscount(basePrice);
            Console.WriteLine("\nGroup Reservation:");
            Console.WriteLine("• 20% discount applied");
            Console.WriteLine("• Discount codes disabled");
        }
        else
        {
            Console.Write("\nDiscount code (optional): ");
            string? code = Console.ReadLine()?.Trim();
            finalPrice = _ctx.discountLogic.Apply(code, basePrice);
        }

        Console.WriteLine($"\nFinal Price: {finalPrice:C}");

        ShowBookingDetails(
            chosenDate.Ticks,
            _ctx.reservationLogic.GenerateOrderNumber(_ctx.loginStatus.CurrentUserInfo),
            session,
            guests,
            finalPrice
        );

        if (UiHelpers.ChoiceHelper("Confirm reservation"))
        {
            _ctx.reservationLogic.CreateSingleTicketBooking(
                session.Id,
                totalGuests,
                _ctx.loginStatus.CurrentUserInfo,
                finalPrice);
        }

        ShowSuccessMessage();
    }

    private ReservationType SelectReservationType()
    {
        List<List<string>> options = new()
        {
            new() { "Normal Reservation (1–10 people)" },
            new() { "Group Reservation (10–30 people)" }
        };

        MainMenu menu = new(options, "Select Reservation Type");
        int[] result = menu.Run();

        return result[0] == 0
            ? ReservationType.Normal
            : ReservationType.Group;
    }

    private DateTime DatePicker()
    {
        int month = DateSelection.monthMenu();
        MainMenu dayMenu = new(
            DateSelection.DaysInSelectedMonth(month),
            DateSelection.Months[month - 1]);

        DateTime date = DateSelection.GetDateFromCoordinate(
            dayMenu.Run(), 2025, month);

        if (date < DateTime.Now)
            throw new InvalidOperationException("Cannot book in the past.");

        if (!_ctx.reservationLogic.CheckForTimeslotsOnDate(date))
            _ctx.reservationLogic.PopulateTimeslots(date);

        return date;
    }

    private SessionModel ShowTimeslotsByDate(List<SessionModel> sessions)
    {
        List<List<string>> options = sessions
            .Select(s => new List<string> { _ctx.reservationLogic.AvailabilityFormatter(s) })
            .ToList();

        MainMenu menu = new(options, "Select Timeslot");
        int[] result = menu.Run();

        return sessions[result[0]];
    }

    private List<int> GuestQuantitySelection()
    {
        MainMenu menu = new(
            new()
            {
                new() { "0-15  : 0" },
                new() { "16-60 : 0" },
                new() { "61+   : 0" }
            },
            "Years | Quantity");

        return menu.Run(1);
    }

    private static void ShowBookingDetails(
        long date,
        string orderNumber,
        SessionModel session,
        List<int> guests,
        double price)
    {
        Console.Clear();
        Console.WriteLine("Booking Details\n");
        Console.WriteLine($"Order: {orderNumber}");

        DateTime sessionDate;

        if (long.TryParse(session.Date.ToString(), out var ticks))
            sessionDate = new DateTime(ticks);
        else
            sessionDate = DateTime.MinValue;

        Console.WriteLine($"Date: {sessionDate:dd-MM-yyyy} {TimeslotOptions[(int)session.Time]}");

        for (int i = 0; i < guests.Count; i++)
            Console.WriteLine($"{AgeOptions[i]}{guests[i]}");

        Console.WriteLine($"\nTotal: {price:C}");
    }

    private static void ShowSuccessMessage()
    {
        Console.WriteLine("\nReservation successful!");
        UiHelpers.Pause();
    }
}
