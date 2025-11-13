    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

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
            else
            {
                Console.WriteLine("Continuing as guest.");//
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
        var userRepo = new UserAccess();
        var loginLogic = new LoginLogic(userRepo);
        new UserLoginUI(loginLogic).StartLogin();

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


    public int GetBookingQuantity(Session session)
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
            .OrderBy(g => g.Key);

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


    public static int GetDateChoice(List<IGrouping<string, Session>> groupedByDate)
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

    public static void ShowSessionsByDate(List<IGrouping<string, Session>> groupedByDate, int dateChoice)
    {
        Console.WriteLine($"\nAvailable Sessions on {groupedByDate[dateChoice].Key}:");
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

        Dictionary<int, Dictionary<int, List<int>>> bookingSelections = new();

        while (true)
        {
            int dateChoice = GetDateChoice(groupedByDate);
            var sessionsOnDate = groupedByDate[dateChoice].ToList();

            ShowSessionsByDate(groupedByDate, dateChoice);
            int sessionChoice = GetSessionChoice(sessionsOnDate);
            Session selectedSession = sessionsOnDate[sessionChoice];

            int bookingQuantity = GetBookingQuantity(selectedSession);

            List<int> ages = new();
            for (int i = 0; i < bookingQuantity; i++)
            {
                while (true)
                {
                    Console.Write($"Enter age for ticket {i + 1}: ");
                    if (int.TryParse(Console.ReadLine(), out int age) && age > 0 && age <= 120)
                    {
                        ages.Add(age);
                        break;
                    }
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }

            if (!bookingSelections.ContainsKey(dateChoice))
                bookingSelections[dateChoice] = new Dictionary<int, List<int>>();
            bookingSelections[dateChoice][selectedSession.Id] = ages;

            bool another = ChoiceHelper("Do you want to book another session?", "Yes, continue.", "No, stop booking.");
            if (!another) break;
        }

        bool confirm = ChoiceHelper("Do you want to confirm your reservation?", "Yes, confirm.", "No, cancel.");
        if (!confirm)
        {
            Console.WriteLine("Reservation cancelled.");
            return;
        }

        string orderNumber = _logic.GenerateOrderNumber(_customerInfo);
        decimal vtotalPrice = 0m;
        decimal totalPrice = 0m;
        int totalTickets = bookingSelections.Values
                            .SelectMany(d => d.Values)
                            .Sum(list => list.Count);

        // Calculate total price => only Claculate ticket age => no offer discount
        Dictionary<int, List<decimal, decimal>> AgeDic = new();

        foreach (var (dateChoice, inner) in bookingSelections)
        {
            foreach (var (sessionId, ages) in inner)
            {
                foreach (int age in ages)
                {
                    (decimal discount, decimal basePrice, decimal finalPrice) = _logic.CreateSingleTicketBooking(
                                            sessionId,
                                            age,
                                            _customerInfo,
                                            orderNumber,
                                            totalTickets);
                    
                    AgeDic[age].Add((discount, finalPrice));
                    
                    vtotalPrice += finalPrice;
                }
            }
        }

        (Dictionary<string, decimal>discountDes, decimal offerDiscount, decimal totalfinalPrice) = _logic.CalculateOfferDiscountedPrice(
                                                                                                                    totalTickets,
                                                                                                                    vtotalPrice,
                                                                                                                    _customerInfo);



        bool payment = ChoiceHelper("Proceed to payment?", "Yes, proceed.", "No, cancel.");
        if (payment)
        {
            PaymentUI.StartPayment(orderNumber, _customerInfo);
            ShowSuccessMessage(orderNumber);
        }
        else
        {
            Console.WriteLine("Payment cancelled. Your reservation is not confirmed.");
        }
    }

    public static void ShowBookingDetails(
        string orderNumber,
        Dictionary<int, Dictionary<int, List<int>>> bookingSelections,
        List<IGrouping<DateTime, Session>> groupedByDate,  // Add this to show actual dates
        Dictionary<int, List<(decimal discount, decimal finalPrice)>> agePricing,  // Add this for ticket details
        Dictionary<string, decimal> discountDescriptions,  // Add this for offer discounts
        decimal totalFinalPrice)
    {
        Console.WriteLine("\n===================== BOOKING SUMMARY =====================");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine("-----------------------------------------------------------");

        foreach (var (dateChoice, sessionBookings) in bookingSelections)
        {
            DateTime actualDate = groupedByDate[dateChoice].Key;
            Console.WriteLine($"\nDate: {actualDate:dddd, MMMM dd, yyyy}");

            foreach (var (sessionId, ages) in sessionBookings)
            {
                // Count ticket types
                int childCount = ages.Count(age => age < 12);
                int seniorCount = ages.Count(age => age >= 65);
                int adultCount = ages.Count(age => age >= 12 && age < 65);

                Console.WriteLine($"\n  Session ID: {sessionId}");
                if (childCount > 0) Console.WriteLine($"    Child Tickets (0-11): {childCount}");
                if (adultCount > 0) Console.WriteLine($"    Adult Tickets (12-64): {adultCount}");
                if (seniorCount > 0) Console.WriteLine($"    Senior Tickets (65+): {seniorCount}");

                if (agePricing.TryGetValue(sessionId, out var ticketPrices))
                {
                    Console.WriteLine($"    Session Subtotal: {ticketPrices.Sum(t => t.finalPrice):C}");
                }
            }
        }

        if (discountDescriptions?.Any() == true)
        {
            Console.WriteLine("\n-----------------------------------------------------------");
            Console.WriteLine("Applied Discounts:");
            foreach (var (description, amount) in discountDescriptions)
            {
                Console.WriteLine($"  {description}: -{amount:C}");
            }
        }

        Console.WriteLine("\n-----------------------------------------------------------");
        Console.WriteLine($"TOTAL AMOUNT: {totalFinalPrice:C}");
        Console.WriteLine("===========================================================\n");
    }



    public static void ShowSuccessMessage(string orderNumber)
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
    }

}
