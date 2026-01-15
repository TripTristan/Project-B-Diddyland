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
    private Dependencies _ctx;
    public UserReservation(Dependencies a) { _ctx = a; }

    public static List<string> AgeOptions = new() { "0-15   : ", "16-60 : ", "61+   : " };
    public static List<string> TimeslotOptions = new() { "09:00-13:00", "13:00-17:00", "17:00-21:00" };


    private (double finalPrice, string? discountInfo) CalculateFinalPrice(List<int> guests,ReservationType reservationType)
    {
        double basePrice = _ctx.reservationLogic.CalculatePriceForGuests(guests);
        var currentUser = _ctx.loginStatus.CurrentUserInfo;

        // Loyalty discount first (no code, no group discount)
        if (_ctx.loyaltyDiscountLogic.CanUseLoyaltyDiscount(currentUser))
        {
            string discountInfo =
                "🎉 Loyalty reward unlocked!\n" +
                "• 5 different visit dates reached\n" +
                "• 50% discount applied\n" +
                "• One-time reward";

            double finalPrice = _ctx.loyaltyDiscountLogic.ApplyAndConsume(currentUser, basePrice);
            return (finalPrice, discountInfo);
        }

        // Group discount next (codes disabled)
        if (reservationType == ReservationType.Group)
        {
            string discountInfo =
                "Group Reservation\n" +
                "• 20% discount applied\n" +
                "• Discount codes disabled";

            double finalPrice = _ctx.reservationLogic.ApplyGroupDiscount(basePrice);
            return (finalPrice, discountInfo);
        }

        // Normal reservation: ask for code
        Console.Write("\nDiscount code (optional): ");
        string? code = Console.ReadLine()?.Trim();

        double priceWithCode = _ctx.discountLogic.Apply(code, basePrice);
        return (priceWithCode, null);
    }

    public void Book()
    {
        // 1) ask for type of reservation
        ReservationType? reservationType = SelectReservationType();
        
        if (reservationType == null)
        {
            return;
        }

        // 2) select date
        DateTime chosenDate = DatePicker();

        // 3) select timeslot
        var sessionsForDate = _ctx.reservationLogic.GetSessionsByDate(chosenDate);
        SessionModel session = ShowTimeslotsByDate(sessionsForDate);

        // 4) select ages
        List<int> guests = GuestQuantitySelection();
        int totalGuests = guests.Sum();

        // validate type vs guest count now (so user gets feedback early)
        _ctx.reservationLogic.ValidateReservationType(totalGuests, (ReservationType)reservationType);

        // 5) ask for discount code (only if allowed) + apply discounts
        var (finalPrice, discountInfo) = CalculateFinalPrice(guests, (ReservationType)reservationType);

        // 6) confirmation screen
        Console.WriteLine($"\nFinal Price: {finalPrice:C}");

        ShowBookingDetails(
            chosenDate.Ticks,
            _ctx.reservationLogic.GenerateOrderNumber(_ctx.loginStatus.CurrentUserInfo),
            session,
            guests,
            finalPrice,
            discountInfo
        );

        if (UiHelpers.ChoiceHelper("Confirm reservation"))
        {
            _ctx.reservationLogic.CreateSingleTicketBooking(
                session.Id,
                totalGuests,
                _ctx.loginStatus.CurrentUserInfo,
                finalPrice
            );
        }

        ShowSuccessMessage();
    }

    public void StartReservationForUser(UserModel user)
    {
        var originalUser = _ctx.loginStatus.CurrentUserInfo;

        _ctx.loginStatus.Login(user);
        Book();
        _ctx.loginStatus.Login(originalUser);
    }

    public SessionModel PickSessionForDate(DateTime date)
    {
        return ShowTimeslotsByDate(
            _ctx.reservationLogic.GetSessionsByDate(date));
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
        double basePrice = _ctx.reservationLogic.CalculatePriceForGuests(guests);

        if (type == ReservationType.Group)
            return _ctx.reservationLogic.ApplyGroupDiscount(basePrice);

        return _ctx.discountLogic.Apply(discountCode, basePrice);
    }

    private ReservationType? SelectReservationType()
    {
        var options = new List<List<string>>
        {
            new() { "Normal Reservation (1–10 people)" },
            new() { "Group Reservation (10–30 people)" },
            new() { "Go Back" }
        };

        var menu = new MainMenu(options, "Select Reservation Type");
        int[] result = menu.Run();

        if (result[0] == 2)
            return null;

        return result[0] == 0
            ? ReservationType.Normal
            : ReservationType.Group;
    }

    private DateTime DatePicker()
    {
        while (true)
        {
            DateTime date = DateSelection.DatePicker();

            if (date.Date < DateTime.Now.Date)
            {
                Console.WriteLine("Cannot book in the past.");
                continue; 
            }

            if (!_ctx.reservationLogic.CheckForTimeslotsOnDate(date))
                _ctx.reservationLogic.PopulateTimeslots(date);

            return date; 
        }
    }


    private SessionModel ShowTimeslotsByDate(List<SessionModel> sessions)
    {
        List<List<string>> options = sessions
            .Select(s => new List<string> { _ctx.reservationLogic.AvailabilityFormatter(s) })
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

        DateTime sessionDate;

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
