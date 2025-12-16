using System;
using System.Collections.Generic;
using System.Linq;

public enum ReservationType
{
    Normal,
    Group
}


public class ReservationUI
{
    public static List<string> AgeOptions = new() { "0-15   : ", "16-60 : ", "61+   : " };
    public static List<string> TimeslotOptions = new() { "09:00-13:00", "13:00-17:00", "17:00-21:00" };

    private readonly ReservationLogic _reservationLogic;
    private readonly PaymentUI _paymentUI;
    private readonly UserLoginUI _loginUI;
    private readonly UiHelpers _ui;
    private readonly SessionAccess _sessionAccess;
    private readonly LoginStatus _loginStatus;
    private readonly FinancialLogic _financialLogic;
    private readonly DiscountCodeLogic _discountLogic;

    public ReservationUI(
        ReservationLogic reservationLogic,
        PaymentUI paymentUI,
        UserLoginUI loginUI,
        UiHelpers ui,
        SessionAccess sessionAccess,
        LoginStatus loginStatus,
        FinancialLogic financialLogic,
        DiscountCodeLogic discountLogic)
    {
        _reservationLogic = reservationLogic;
        _paymentUI = paymentUI;
        _loginUI = loginUI;
        _ui = ui;
        _sessionAccess = sessionAccess;
        _loginStatus = loginStatus;
        _financialLogic = financialLogic;
        _discountLogic = discountLogic;
    }

    public void StartReservation()
    {
        ReservationType reservationType = SelectReservationType();

        DateTime chosenDate = DatePicker();
        SessionModel session = ShowTimeslotsByDate(
            _reservationLogic.GetSessionsByDate(chosenDate));

        List<int> guests = GuestQuantitySelection();
        int totalGuests = guests.Sum();

        _reservationLogic.ValidateReservationType(totalGuests, reservationType);

        double basePrice = _reservationLogic.CalculatePriceForGuests(guests);
        double finalPrice;

        if (reservationType == ReservationType.Group)
        {
            finalPrice = _reservationLogic.ApplyGroupDiscount(basePrice);
            Console.WriteLine("\nGroup Reservation:");
            Console.WriteLine("• 20% discount applied");
            Console.WriteLine("• Discount codes disabled");
        }
        else
        {
            Console.Write("\nDiscount code (optional): ");
            string? code = Console.ReadLine()?.Trim();
            finalPrice = _discountLogic.Apply(code, basePrice);
        }

        Console.WriteLine($"\nFinal Price: {finalPrice:C}");

        ShowBookingDetails(
            chosenDate.Ticks,
            _reservationLogic.GenerateOrderNumber(_loginStatus.CurrentUserInfo),
            session,
            guests,
            finalPrice
        );

        if (UiHelpers.ChoiceHelper("Confirm reservation"))
        {
            _reservationLogic.CreateSingleTicketBooking(
                session.Id,
                totalGuests,
                _loginStatus.CurrentUserInfo,
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
        int month = FinancialMenu.monthMenu();
        MainMenu dayMenu = new(
            FinancialMenu.DaysInSelectedMonth(month),
            FinancialMenu.Months[month - 1]);

        DateTime date = _financialLogic.GetDateFromCoordinate(
            dayMenu.Run(), 2025, month);

        if (date < DateTime.Now)
            throw new InvalidOperationException("Cannot book in the past.");

        if (!_reservationLogic.CheckForTimeslotsOnDate(date))
            _reservationLogic.PopulateTimeslots(date);

        return date;
    }

    private SessionModel ShowTimeslotsByDate(List<SessionModel> sessions)
    {
        List<List<string>> options = sessions
            .Select(s => new List<string> { _reservationLogic.AvailabilityFormatter(s) })
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
