    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ReservationUI
    {
        private static UserModel? _customerInfo;

    public static int week = 0;

    public static void StartReservation()
    {
            _customerInfo = LoginStatus.CurrentUserInfo;
            Console.WriteLine("=== Reservation ===");

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
                }
                else
                {
                    Console.WriteLine("Continuing as guest.");
                // can here email in //
                }
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

            if (choice == "y") return true;
            if (choice == "n") return false;

            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        }
    }


        public static int GetBookingQuantity(Session session) // Get and verify booking quantity
        {
            while (true)
            {
                Console.Write("Enter the number of bookings you want to make: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    if (ReservationLogic.CanBookSession(session.Id, quantity))
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
                .Where(s => !IsFastPassSlot(s.Time))    // ✅ skip fastpass-only sessions
                .GroupBy(s => s.Time.Trim().ToLower())  // remove duplicate labels
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
        var t = time.Trim().ToLowerInvariant();
        return TimeSpan.TryParse(t, out _); 
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
                Console.WriteLine($"{i}. Time: {s.Time}, Available Spots: {SessionAccess.GetCapacityBySession(s) - s.CurrentBookings}");
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



        public static void SelectAndProcessSession(List<Session> sessions) 
        {
            var groupedByDate = sessions.GroupBy(s => s.Date).ToList();
            Dictionary<int, List<int>> bookingSelections = new(); // sessionId -> quantity
            int dateChoice = GetDateChoice(groupedByDate);
            var sessionsOnDate = groupedByDate[dateChoice].ToList();

            ShowSessionsByDate(groupedByDate, dateChoice);
            int sessionChoice = GetSessionChoice(sessionsOnDate);
            Session selectedSession = sessionsOnDate[sessionChoice];
            int bookingQuantity = GetBookingQuantity(selectedSession);

            while (true)
            {
                List<int> ages = new();
                // age for every ticket
                // for (int i = 0; i < bookingQuantity; i++)
                // {
                while (ages.Count < bookingQuantity)
                {
                    Console.Write($"Enter age for ticket {ages.Count + 1}: ");
                    if (int.TryParse(Console.ReadLine(), out int age) && age > 0 && age <= 120)
                        ages.Add(age);

                    else
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
            // }

            bookingSelections[selectedSession.Id] = ages;

                // bool another = ChoiceHelper("Do you want to book another session?", "Yes, continue.", "No, stop booking.");
                // if (!another) 
                break;
            }


            bool confirm = ChoiceHelper("Do you want to confirm your reservation?", "Yes, confirm.", "No, cancel.");
        if (confirm)
        {
            string orderNumber = ReservationLogic.GenerateOrderNumber(_customerInfo);
            decimal totalPrice = 0; // for discount calculation

            foreach (int age in bookingSelections[selectedSession.Id])
            {
                Console.WriteLine("part1");
                decimal singlePrice = ReservationLogic.CreateSingleTicketBooking(selectedSession.Id, age, _customerInfo, orderNumber, bookingQuantity);
                totalPrice += singlePrice;
            }

            ShowBookingDetails(orderNumber, bookingSelections, totalPrice);

            bool payment = ChoiceHelper("Proceed to payment?", "Yes, proceed.", "No, cancel.");
            if (payment)
            {
                PaymentUI.StartPayment(orderNumber, _customerInfo);
                ShowSuccessMessage(orderNumber);
                ShowBookingDetails(orderNumber, bookingSelections, totalPrice);
            }
            else
            {
                Console.WriteLine("Payment cancelled. Your reservation is not confirmed.");

            }
            ShowBookingDetails(orderNumber, bookingSelections, totalPrice);

        }
        else
        {
            Console.WriteLine("Reservation cancelled.");
            // go back to main menu // Maybe
            return;
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
    week = 0;  // Initialize starting week (current week)
    int choice = 1;

    while (choice != 0)
    {
        Console.Clear();
        

        // Calculate the start and end date for the current week
        DateTime currentDate = DateTime.Now;
        DateTime startOfWeek = currentDate.AddDays(week * 7 - (int)currentDate.DayOfWeek);  // Calculate the start of the current week
        DateTime endOfWeek = startOfWeek.AddDays(7);  // End of the week is 7 days after start

        // Display the sessions for the calculated week
        DisplayDates(sessions, startOfWeek, endOfWeek);

        bool validInput = int.TryParse(Console.ReadLine(), out choice);
        if (!validInput)
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        // Switch block for user choices
        switch (choice)
        {
            case 1:
                week++;  // Move to next week
                break;
            case 2:
                if (week > 0)
                {
                    week--;  // Move to previous week
                }
                else
                {
                        Console.WriteLine("You can't book into the past");
                    
                }
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