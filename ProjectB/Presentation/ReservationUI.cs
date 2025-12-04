using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationUI
{
    private readonly ReservationLogic _reservationLogic;
    private readonly PaymentUI _paymentUI;
    private readonly UserLoginUI _loginUI;
    private readonly UiHelpers _ui;
    private readonly SessionAccess _sessionAccess;
    private readonly LoginStatus _loginStatus;

    private UserModel? _customerInfo;
    private int _week = 0;

    public ReservationUI(
        ReservationLogic reservationLogic,
        PaymentUI paymentUI,
        UserLoginUI loginUI,
        UiHelpers ui,
        SessionAccess sessionAccess,
        LoginStatus loginStatus)
    {
        _reservationLogic = reservationLogic;
        _paymentUI = paymentUI;
        _loginUI = loginUI;
        _ui = ui;
        _sessionAccess = sessionAccess;
        _loginStatus = loginStatus;
    }

    public void StartReservation()
    {
        _customerInfo = _loginStatus.CurrentUserInfo;

        Console.WriteLine("=== Reservation ===");

        if (_customerInfo == _loginStatus.Guest)
        {
            bool guest = ChoiceHelper(
                "You are not logged in. Continue as guest?",
                "Yes, continue as guest.",
                "No, I want to log in."
            );

            if (!guest)
            {
                _loginUI.StartLogin();
                _customerInfo = _loginStatus.CurrentUserInfo;
            }
            else
            {
                Console.WriteLine("Continuing as guest.");
            }
        }

        var sessions = _reservationLogic.GetAvailableSessions();

        if (sessions.Count == 0)
        {
            Console.WriteLine("No available sessions.");
            return;
        }

        WeekBrowser(sessions);
        SelectAndProcessSession(sessions);
    }

    private bool ChoiceHelper(string message, string yesOption, string noOption)
    {
        while (true)
        {
            Console.WriteLine($"{message} (y/n):\n y - {yesOption}\n n - {noOption}");
            string choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y") return true;
            if (choice == "n") return false;

            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        }
    }

    private int GetBookingQuantity(Session session)
    {
        while (true)
        {
            Console.Write("Enter the number of bookings you want: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                if (_reservationLogic.CanBookSession(session.Id, quantity))
                    return quantity;

                Console.WriteLine("Not enough available seats. Please try again.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a positive number.");
            }
        }
    }

    private void DisplayDates(List<Session> sessions, DateTime startOfWeek, DateTime endOfWeek)
    {
        Console.WriteLine("\nAvailable sessions:");

        var orderedGroups = sessions
            .Where(s =>
            {
                bool ok = DateTime.TryParse(s.Date, out var dt);
                return ok && dt >= startOfWeek && dt < endOfWeek;
            })
            .GroupBy(s => DateTime.Parse(s.Date))
            .OrderBy(g => g.Key);

        int seshId = 1;

        foreach (var group in orderedGroups)
        {
            var orderedInDay = group
                .Where(s => !IsFastPassSlot(s.Time))
                .GroupBy(s => s.Time.Trim().ToLower())
                .Select(g => g.First())
                .OrderBy(s => s.Time);

            Console.WriteLine($"\nDate: {group.Key:yyyy-MM-dd}\nTime Slots:");

            foreach (var s in orderedInDay)
            {
                int capacity = _sessionAccess.GetCapacityBySession(s);
                int available = capacity - s.CurrentBookings;

                Console.WriteLine(
                    $"Date: {group.Key:yyyy-MM-dd}, " +
                    $"Time: {s.Time}, " +
                    $"Available Spots: {available}"
                );
            }

            Console.WriteLine();
            seshId++;
        }

        _ui.WriteHeader($"      < {_week} >      ");
        Console.WriteLine("0) This week\n1) Next week\n2) Previous week");
    }

    private bool IsFastPassSlot(string time)
    {
        if (string.IsNullOrWhiteSpace(time)) return false;
        return TimeSpan.TryParse(time.Trim().ToLower(), out _);
    }

    private int GetDateChoice(List<IGrouping<string, Session>> groupedByDate)
    {
        while (true)
        {
            Console.Write("Please select a date number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 0 &&
                choice < groupedByDate.Count)
                return choice;

            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    private void ShowSessionsByDate(List<IGrouping<string, Session>> groupedByDate, int dateChoice)
    {
        Console.WriteLine($"\nAvailable Sessions on {groupedByDate[dateChoice].Key}:");

        var sessionsOnDate = groupedByDate[dateChoice].ToList();

        for (int i = 0; i < sessionsOnDate.Count; i++)
        {
            var s = sessionsOnDate[i];

            int capacity = _sessionAccess.GetCapacityBySession(s);
            int available = capacity - s.CurrentBookings;

            Console.WriteLine($"{i}. Time: {s.Time}, Available Spots: {available}");
        }
    }

    private int GetSessionChoice(List<Session> sessionsOnDate)
    {
        while (true)
        {
            Console.Write("Select a session number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 0 &&
                choice < sessionsOnDate.Count)
            {
                return choice;
            }

            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    private void SelectAndProcessSession(List<Session> sessions)
    {
        var groupedByDate = sessions.GroupBy(s => s.Date).ToList();
        Dictionary<int, List<int>> bookingSelections = new();

        int dateChoice = GetDateChoice(groupedByDate);
        var sessionsOnDate = groupedByDate[dateChoice].ToList();

        ShowSessionsByDate(groupedByDate, dateChoice);

        int sessionChoice = GetSessionChoice(sessionsOnDate);
        var selectedSession = sessionsOnDate[sessionChoice];

        int bookingQuantity = GetBookingQuantity(selectedSession);

        List<int> ages = new();
        while (ages.Count < bookingQuantity)
        {
            Console.Write($"Enter age for ticket {ages.Count + 1}: ");
            if (int.TryParse(Console.ReadLine(), out int age) &&
                age > 0 && age <= 120)
            {
                ages.Add(age);
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        bookingSelections[selectedSession.Id] = ages;

        bool confirm = ChoiceHelper("Do you want to confirm your reservation?", "Yes, confirm.", "No, cancel.");
        if (!confirm)
        {
            Console.WriteLine("Reservation cancelled.");
            return;
        }

        string orderNumber = _reservationLogic.GenerateOrderNumber(_customerInfo);
        int representativeAge = ages[0];

        decimal totalPrice = _reservationLogic.CreateSingleTicketBooking(
            selectedSession.Id,
            representativeAge,
            _customerInfo,
            orderNumber,
            bookingQuantity
        );

        ShowBookingDetails(orderNumber, bookingSelections, totalPrice);

        bool payment = ChoiceHelper("Proceed to payment?", "Yes, proceed.", "No, cancel.");
        if (payment)
        {
            _paymentUI.StartPayment(orderNumber, _customerInfo);
            Console.WriteLine("Reservation successful!");
            ShowBookingDetails(orderNumber, bookingSelections, totalPrice);
        }
        else
        {
            Console.WriteLine("Payment cancelled. Reservation NOT confirmed.");
        }
    }

    private void ShowBookingDetails(string orderNumber, Dictionary<int, List<int>> bookingDetails, decimal totalPrice)
    {
        Console.WriteLine("---------------------");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine("Booking Details:");

        foreach (var (sessionId, ages) in bookingDetails)
        {
            int child = ages.Count(a => a < 12);
            int senior = ages.Count(a => a >= 65);
            int adult = ages.Count - child - senior;

            Console.WriteLine($"Session ID: {sessionId}");
            Console.WriteLine($"  Child Tickets:  {child}");
            Console.WriteLine($"  Senior Tickets: {senior}");
            Console.WriteLine($"  Adult Tickets:  {adult}");
        }

        Console.WriteLine($"Total Price: {totalPrice:C}");
    }

    private void WeekBrowser(List<Session> sessions)
    {
        _week = 0;
        int choice = 1;

        while (choice != 0)
        {
            Console.Clear();

            DateTime now = DateTime.Now;
            DateTime startOfWeek = now.AddDays(_week * 7 - (int)now.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            DisplayDates(sessions, startOfWeek, endOfWeek);

            bool ok = int.TryParse(Console.ReadLine(), out choice);
            if (!ok)
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1: _week++; break;
                case 2:
                    if (_week > 0)
                        _week--;
                    else
                        Console.WriteLine("Cannot browse into the past.");
                    break;

                case 0: break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
