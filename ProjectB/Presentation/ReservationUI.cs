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
    public static List<string> AgeOptions = new()
    {
        "0-15   : ",
        "16-60 : ",
        "61+   : "
    };

    public static List<string> TimeslotOptions = new()
    {
        "09:00-13:00",
        "13:00-17:00",
        "17:00-21:00"
    };

    private readonly ReservationLogic _reservationLogic;
    private readonly PaymentUI _paymentUI;
    private readonly UserLoginUI _loginUI;
    private readonly UiHelpers _ui;
    private readonly SessionAccess _sessionAccess;
    private readonly LoginStatus _loginStatus;
    private readonly FinancialLogic _financialLogic;
    private readonly DiscountCodeLogic _discountLogic;
    private readonly DatePickerUI _datePicker;
    private readonly LoyaltyDiscountLogic _loyaltyDiscountLogic;

    public ReservationUI(
        ReservationLogic reservationLogic,
        PaymentUI paymentUI,
        UserLoginUI loginUI,
        UiHelpers ui,
        SessionAccess sessionAccess,
        LoginStatus loginStatus,
        FinancialLogic financialLogic,
        DiscountCodeLogic discountLogic,
        DatePickerUI datePicker,
        LoyaltyDiscountLogic loyaltyDiscountLogic)
    {
        _reservationLogic = reservationLogic;
        _paymentUI = paymentUI;
        _loginUI = loginUI;
        _ui = ui;
        _sessionAccess = sessionAccess;
        _loginStatus = loginStatus;
        _financialLogic = financialLogic;
        _discountLogic = discountLogic;
        _datePicker = datePicker;
        _loyaltyDiscountLogic = loyaltyDiscountLogic;
    }

    public void StartReservation()
    {
        ReservationType reservationType = SelectReservationType();

        DateTime chosenDate = _datePicker.PickDate();

        SessionModel session = ShowTimeslotsByDate(
            _reservationLogic.GetSessionsByDate(chosenDate));

        List<int> guests = GuestQuantitySelection();
        int totalGuests = guests.Sum();

        _reservationLogic.ValidateReservationType(totalGuests, reservationType);

        double basePrice = _reservationLogic.CalculatePriceForGuests(guests);
        double finalPrice;

        var currentUser = _loginStatus.CurrentUserInfo;

        string? discountInfo = null;

        if (_loyaltyDiscountLogic.CanUseLoyaltyDiscount(currentUser))
        {
            discountInfo =
                "🎉 Loyalty reward unlocked!\n" +
                "• 5 different visit dates reached\n" +
                "• 50% discount applied\n" +
                "• One-time reward";

            finalPrice = _loyaltyDiscountLogic.ApplyAndConsume(
                currentUser,
                basePrice
            );
        }
        else if (reservationType == ReservationType.Group)
        {
            discountInfo =
                "Group Reservation\n" +
                "• 20% discount applied\n" +
                "• Discount codes disabled";

            finalPrice = _reservationLogic.ApplyGroupDiscount(basePrice);
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
            _reservationLogic.GenerateOrderNumber(currentUser),
            session,
            guests,
            finalPrice,
            discountInfo
        );

        if (UiHelpers.ChoiceHelper("Confirm reservation"))
        {
            _reservationLogic.CreateSingleTicketBooking(
                session.Id,
                totalGuests,
                currentUser,
                finalPrice
            );
        }

        ShowSuccessMessage();
    }

    public void StartReservationForUser(UserModel user)
    {
        var originalUser = _loginStatus.CurrentUserInfo;

        _loginStatus.Login(user);
        StartReservation();
        _loginStatus.Login(originalUser);
    }

    public SessionModel PickSessionForDate(DateTime date)
    {
        return ShowTimeslotsByDate(
            _reservationLogic.GetSessionsByDate(date));
    }

    public List<int> PickGuestQuantities()
    {
        return GuestQuantitySelection();
    }

    public double CalculatePriceFromGuests(
        List<int> guests,
        ReservationType type,
        string? discountCode = null)
    {
        double basePrice = _reservationLogic.CalculatePriceForGuests(guests);

        if (type == ReservationType.Group)
            return _reservationLogic.ApplyGroupDiscount(basePrice);

        return _discountLogic.Apply(discountCode, basePrice);
    }

    private ReservationType SelectReservationType()
    {
        var options = new List<List<string>>
        {
            new() { "Normal Reservation (1–10 people)" },
            new() { "Group Reservation (10–30 people)" }
        };

        var menu = new MainMenu(options, "Select Reservation Type");
        int[] result = menu.Run();

        return result[0] == 0
            ? ReservationType.Normal
            : ReservationType.Group;
    }

    private SessionModel ShowTimeslotsByDate(List<SessionModel> sessions)
    {
        var options = sessions
            .Select(s => new List<string>
            {
                _reservationLogic.AvailabilityFormatter(s)
            })
            .ToList();

        var menu = new MainMenu(options, "Select Timeslot");
        int[] result = menu.Run();

        return sessions[result[0]];
    }

    private List<int> GuestQuantitySelection()
    {
        var menu = new MainMenu(
            new()
            {
                new() { "0-15  : 0" },
                new() { "16-60 : 0" },
                new() { "61+   : 0" }
            },
            "Years | Quantity"
        );

        return menu.Run(1);
    }

    private static void ShowBookingDetails(
        long dateTicks,
        string orderNumber,
        SessionModel session,
        List<int> guests,
        double price,
        string? discountInfo)
    {
        Console.Clear();
        Console.WriteLine("Booking Details\n");

        if (!string.IsNullOrEmpty(discountInfo))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(discountInfo);
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine($"Order: {orderNumber}");

        DateTime date = new DateTime(dateTicks);
        Console.WriteLine(
            $"Date: {date:dd-MM-yyyy} {TimeslotOptions[(int)session.Time]}");

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
