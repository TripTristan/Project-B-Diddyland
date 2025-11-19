using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class ReservationUI
{
    private static UserModel? _customerInfo;
    public static int week = 0;

    public static void StartReservation()
    {
        _customerInfo = LoginStatus.CurrentUserInfo;
        Console.Clear();
        Console.WriteLine("=== Reservation System ===\n");

        if (_customerInfo == LoginStatus.guest)
        {
            bool guest = ChoiceHelper(
                "You are not logged in. Continue as guest?",
                "Yes, continue as guest.",
                "No, I want to log in."
            );
            if (!guest)
            {
                UserLoginUI.StartLogin();
                _customerInfo = LoginStatus.CurrentUserInfo;
                if (_customerInfo == null || _customerInfo == LoginStatus.guest)
                {
                    Console.WriteLine("Login required to continue.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Continuing as guest.");
            }
        }

        bool groupReservation = ChoiceHelper(
            "Would you like to make a group reservation?\n For Example a school/organization.\n Buying more than 30 at once?\nThen you can choose a group.\nGroup tickets offer even bigger discounts!",
            "Yes",
            "No"
        );
        if (groupReservation)
        {
            GroupReservationUI.StartGroupReservation(_customerInfo);
            return;
        }

        List<Session> sessions = ReservationLogic.GetAvailableSessions();
        if (sessions.Count == 0)
        {
            Console.WriteLine("No available sessions.");
            return;
        }

        WeekBrowser(sessions);
        SelectAndProcessSession(sessions);
    }

    public static bool ChoiceHelper(string message, string yesOption, string noOption)
    {
        while (true)
        {
            Console.WriteLine($"{message} (y/n):\n y - {yesOption}\n n - {noOption}");
            string choice = Console.ReadLine()?.Trim().ToLower();
            if (choice == "yes") return true;
            if (choice == "no") return false;
            Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
        }
    }

    public static int GetBookingQuantity(Session session)
    {
        while (true)
        {
            Console.Write("Enter the number of bookings you want to make: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                if (ReservationLogic.CanBookSession(session.Id, quantity))
                    return quantity;
                else
                    Console.WriteLine("Not enough available seats. Please try again.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a positive number.");
            }
        }
    }

    public static void DisplayDates(List<Session> sessions, DateTime startOfWeek, DateTime endOfWeek)
    {
        Console.WriteLine("\nAvailable sessions:");

        var orderedGroups = sessions
            .Where(s =>
            {
                DateTime sessionDate;
                bool isValidDate = DateTime.TryParse(s.Date, out sessionDate);
                return isValidDate && sessionDate >= startOfWeek && sessionDate < endOfWeek;
            })
            .GroupBy(s => DateTime.Parse(s.Date))
            .OrderBy(g => g.Key);

        int seshid = 1;
        foreach (var group in orderedGroups)
        {
            var orderedInDay = group
                .Where(s => !IsFastPassSlot(s.Time))
                .GroupBy(s => s.Time.Trim().ToLower())
                .Select(g => g.First())
                .OrderBy(s => s.Time);

            Console.WriteLine($"\nDate: {group.Key:yyyy-MM-dd}\nTime Slots:");
            Console.WriteLine($"{seshid * (week + 1)}) ");
            foreach (var s in orderedInDay)
            {
                DateTime sessionDate = DateTime.Parse(s.Date);
                Console.WriteLine(
                    $"Date: {sessionDate:yyyy-MM-dd}, " +
                    $"Time: {s.Time}, " +
                    $"Available Spots: {SessionAccess.GetCapacityBySession(s) - s.CurrentBookings}");
            }
            Console.WriteLine();
            seshid++;
        }

        UiHelpers.WriteHeader($"      < {week} >      ");
        Console.WriteLine($"0) This week\n1) View next week\n2) View last week");
    }

    private static bool IsFastPassSlot(string time)
    {
        if (string.IsNullOrWhiteSpace(time)) return false;
        return TimeSpan.TryParse(time.Trim().ToLowerInvariant(), out _);
    }

    public static int GetDateChoice(List<IGrouping<string, Session>> groupedByDate)
    {
        while (true)
        {
            Console.Write("Please select a date number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < groupedByDate.Count)
                return choice;
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    public static void ShowSessionsByDate(List<IGrouping<string, Session>> groupedByDate, int dateChoice)
    {
        Console.WriteLine($"\nAvailable Sessions on {groupedByDate[dateChoice].Key}:");
        var sessionsOnDate = groupedByDate[dateChoice].ToList();
        for (int i = 0; i < sessionsOnDate.Count; i++)
        {
            var s = sessionsOnDate[i];
            Console.WriteLine($"{i}. Time: {s.Time}, Available Spots: {SessionAccess.GetCapacityBySession(s) - s.CurrentBookings}");
        }
    }

    public static int GetSessionChoice(List<Session> sessionsOnDate)
    {
        while (true)
        {
            Console.Write("Select a session number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < sessionsOnDate.Count)
                return choice;
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    public static void SelectAndProcessSession(List<Session> sessions)
    {
        var groupedByDate = sessions.GroupBy(s => s.Date).ToList();
        int dateChoice = GetDateChoice(groupedByDate);
        var sessionsOnDate = groupedByDate[dateChoice].ToList();

        ShowSessionsByDate(groupedByDate, dateChoice);
        int sessionChoice = GetSessionChoice(sessionsOnDate);
        Session selectedSession = sessionsOnDate[sessionChoice];
        int bookingQuantity = GetBookingQuantity(selectedSession);

        List<(int sessionId, int age)> cart = new();
        while (cart.Count < bookingQuantity)
        {
            Console.Write($"Enter age for ticket {cart.Count + 1}: ");
            if (int.TryParse(Console.ReadLine(), out int age) && age > 0 && age <= 120)
                cart.Add((selectedSession.Id, age));
            else
                Console.WriteLine("Invalid age.");
        }

        var summary = DiscountLogic.ApplyAllDiscounts(cart, _customerInfo, promoCode: null);

        bool confirm = ChoiceHelper(
            $"Total price after discounts: {summary.FinalTotal:C}  (original {summary.OriginalSubTotal:C}). Confirm?",
            "Yes, confirm.",
            "No, cancel.");
        if (!confirm) return;

        string orderNumber = ReservationLogic.GenerateOrderNumber(_customerInfo);

        foreach (var (sessionId, age) in cart)
        {
            var ticket = summary.TicketDetails.First(t => t.SessionId == sessionId && t.Age == age);
            ReservationLogic.CreateSingleTicketBooking(sessionId, age, _customerInfo, orderNumber, ticket.FinalPrice);
        }

        ShowBookingDetails(orderNumber, summary);
        bool payment = ChoiceHelper("Proceed to payment?", "Yes", "No");
        if (payment)
        {
            PaymentUI.StartPayment(orderNumber, _customerInfo);
            ShowSuccessMessage(orderNumber);
        }
        else
        {
            Console.WriteLine("Payment cancelled.");
        }
    }

    public static void ShowBookingDetails(string orderNumber, Dictionary<int, List<int>> bookingDetails, decimal totalPrice)
    {
        Console.WriteLine("---------------------");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine("Booking Details:");

        var sessionCount = new Dictionary<int, List<int>>();

        foreach (var (sessionId, ages) in bookingDetails)
        {
            int child = 0, senior = 0, adult = 0;
            foreach (int age in ages)
            {
                if (age < 12) child++;
                else if (age >= 65) senior++;
                else adult++;
            }
            sessionCount[sessionId] = new List<int> { child, senior, adult };
        }

        foreach (var (sessionId, _) in bookingDetails)
        {
            Console.WriteLine($"Session ID: {sessionId}");
            Console.WriteLine($"  Child Tickets:  {sessionCount[sessionId][0]}");
            Console.WriteLine($"  Senior Tickets: {sessionCount[sessionId][1]}");
            Console.WriteLine($"  Adult Tickets:  {sessionCount[sessionId][2]}");
        }

        Console.WriteLine($"Total Price: {totalPrice:C}");
    }

    public static void ShowSuccessMessage(string orderNumber)
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
    }

    public static void WeekBrowser(List<Session> sessions)
    {
        week = 0;
        int choice = 1;

        while (choice != 0)
        {
            Console.Clear();
            DateTime currentDate = DateTime.Now;
            DateTime startOfWeek = currentDate.AddDays(week * 7 - (int)currentDate.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            DisplayDates(sessions, startOfWeek, endOfWeek);

            bool validInput = int.TryParse(Console.ReadLine(), out choice);
            if (!validInput)
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    week++;
                    break;
                case 2:
                    if (week > 0)
                        week--;
                    else
                        Console.WriteLine("You can't book into the past");
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please choose a valid option.");
                    break;
            }
        }
    }
}