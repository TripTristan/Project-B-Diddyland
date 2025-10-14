using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationUI
{
    private readonly ReservationLogic _logic;
    private readonly UserModel? _customerInfo = LoginStatus.CurrentUserInfo;

    public ReservationUI(ReservationLogic logic)
    {
        _logic = logic;
    }


    public void StartReservation()
    {
        Console.WriteLine("=== Reservation ===");

        if (_customerInfo == null)
        {
            bool guest = ChoiceHelper(
                "You are not logged in. Continue as guest?",
                "Yes, continue as guest.",
                "No, I want to log in."
            );
            if (!guest)
            {
                LoginCustomer();
            }
        }

        List<Session> sessions = _logic.GetAvailableSessions();
        if (sessions.Count == 0)
        {
            Console.WriteLine("No available sessions.");
            return;
        }

        DisplayDates(sessions);
        SelectAndProcessSession(sessions);
    }


    public static void LoginCustomer()
    {
        UserLoginUI.StartLogin();
    }


    public static bool ChoiceHelper(string message, string yesOption, string noOption)
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


    public int GetBookingQuantity(Session session) // Get and verify booking quantity
    {
        while (true)
        {
            Console.Write("Enter the number of bookings you want to make: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                if (_logic.CanBookSession(session.Id, quantity))
                {
                    return quantity;
                }
                else
                {
                    Console.WriteLine("Not enough available seats. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a positive number.");
            }
        }
    }


    public static void DisplayDates(List<Session> sessions)
    {
        Console.WriteLine("\nAvailable sessions:");

        var orderedGroups = sessions
            .GroupBy(s => s.Date)   
            .OrderBy(g => g.Key);   // ascending

        int index = 0;
        foreach (var group in orderedGroups)
        {
            Console.WriteLine($"\nDate: {group.Key:yyyy-MM-dd}\nTime Slots:");
            var orderedInDay = group.OrderBy(s => s.Time);

            foreach (var s in orderedInDay)
            {
                Console.WriteLine(
                    $"{index++}. Date: {s.Date:yyyy-MM-dd}, " +
                    $"Time: {s.Time}, " +
                    $"Available Spots: {s.MaxCapacity - s.CurrentBookings}\n");
            }
        }
    }


    public static int GetDateChoice(List<IGrouping<DateOnly, Session>> groupedByDate)
    {
        while (true)
        {
            Console.Write("Please select a date number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < groupedByDate.Count)
            {
                return choice;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }


    public static void ShowSessionsByDate(List<IGrouping<DateOnly, Session>> groupedByDate, int dateChoice)
    {
        Console.WriteLine($"\nAvailable Sessions on {groupedByDate[dateChoice].Key:yyyy-MM-dd}:");
        var sessionsOnDate = groupedByDate[dateChoice].ToList();
        for (int i = 0; i < sessionsOnDate.Count; i++)
        {
            var s = sessionsOnDate[i];
            Console.WriteLine($"{i}. Time: {s.Time}, Available Spots: {s.MaxCapacity - s.CurrentBookings}");
        }
    }


    public static int GetSessionChoice(List<Session> sessionsOnDate)
    {
        while (true)
        {
            Console.Write("Select a session number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < sessionsOnDate.Count)
            {
                return choice;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }



    public void SelectAndProcessSession(List<Session> sessions)
    {
        var groupedByDate = sessions.GroupBy(s => s.Date).ToList();
        Dictionary<int, int> bookingSelections = new(); // sessionId -> quantity

        while (true)
        {
            int dateChoice = GetDateChoice(groupedByDate);
            var sessionsOnDate = groupedByDate[dateChoice].ToList();

            ShowSessionsByDate(groupedByDate, dateChoice);
            int sessionChoice = GetSessionChoice(sessionsOnDate);
            Session selectedSession = sessionsOnDate[sessionChoice];

            int bookingQuantity = GetBookingQuantity(selectedSession);
            bookingSelections[selectedSession.Id] = bookingQuantity;

            bool another = ChoiceHelper("Do you want to book another session?", "Yes, continue.", "No, stop.");
            if (!another) break;
        }

        bool confirm = ChoiceHelper("Do you want to confirm your reservation?", "Yes, confirm.", "No, cancel.");
        if (confirm)
        {
            string orderNumber = _logic.GenerateOrderNumber(_customerInfo); // unikue order nr// shopping-car 

            foreach (var (sessionId, quantity) in bookingSelections)
            {
                _logic.CreateBooking(sessionId, quantity, _customerInfo, orderNumber);
            }

            ShowSuccessMessage(orderNumber);
            ShowBookingDetails("Multiple Orders", bookingSelections);
        }
        else
        {
            Console.WriteLine("Reservation cancelled.");
        }
    }


    public static void ShowBookingDetails(string orderNumber, Dictionary<int, int> bookingDetails)
    {
        Console.WriteLine("---------------------");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine("Booking Details:");
        foreach (var (sessionId, quantity) in bookingDetails)
        {
            Console.WriteLine($"Session ID {sessionId}: {quantity} bookings");
        }
        Console.WriteLine("---------------------");
    }


    public static void ShowSuccessMessage(string orderNumber)
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
        Console.WriteLine($"Order Number: {orderNumber}");
    }
}