    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;

    public static class ReservationUI
    {
        private static UserModel? _customerInfo;

    
    // public static int week = Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    public static int week = 0;
    public static void StartReservation()
    {
            _customerInfo = LoginStatus.CurrentUserInfo;
            Console.WriteLine("=== Reservation ===");

            if (_customerInfo == LoginStatus.guest)
            {
                bool guest = UiHelpers.ChoiceHelper("You are not logged in. Continue as guest?");
                if (!guest)
                {
                    UserLoginUI.StartLogin();
                }
                else
                {
                    Console.WriteLine("Continuing as guest.");
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


    public static string DisplayDates(List<Session> sessions)
    {
        int month = FinancialMenu.monthMenu();
        MainMenu DayChoice = new MainMenu(FinancialMenu.DaysInSelectedMonth(month), "Select the date:");
        DateTime DateSelected = FinancialLogic.GetDateFromCoordinate(DayChoice.Run(), 2025, month);
        Console.WriteLine("\nAvailable sessions:");

        var orderedGroups = sessions
            .Where(s =>
            {

                DateTime sessionDate;
                bool isValidDate = DateTime.TryParse(s.Date, out sessionDate);
                return isValidDate;
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
            foreach (Session s in orderedInDay)
            {
                DateTime sessionDate = DateTime.Parse(s.Date);
                if (sessionDate.ToString("ddMMyyyy") == DateSelected.ToString("ddMMyyyy"))
                {
                    Console.WriteLine(
                        $"Date: {sessionDate:yyyy-MM-dd}, " +
                        $"Time: {s.Time}, " +
                        $"Available Spots: {SessionAccess.GetCapacityBySession(s) - s.CurrentBookings}");
                }
            }
            Console.WriteLine();
        }
        return $"Select a week";
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

        public static int ShowSessionsByDate(List<IGrouping<string, Session>> groupedByDate, int dateChoice)
        {
            Console.WriteLine($"\nAvailable Sessions on {groupedByDate[dateChoice].Key}:");
            var sessionsOnDate = groupedByDate[dateChoice].ToList();
            string Prompt = "Select a session";
            List<List<string>> Options = new();

            foreach (Session sesh in sessionsOnDate)
            {
                Options.Add(new List<string> {$"{sesh.Date}  {sesh.Time} {ReservationLogic.GetAttractionNameByAttractionID(sesh.AttractionID)} {ReservationLogic.GetAvailableSpotsForSession(sesh)}"});
            }
            MainMenu Menu = new MainMenu(Options, Prompt);
            UiHelpers.Pause();
            int[] selectedIndex = Menu.Run();
            
            Console.ResetColor();

            return selectedIndex[0];
        }



        public static int GetSessionChoice(List<Session> sessionsOnDate)
        {
            string Prompt = "Select a session";
            List<List<string>> Options = new();

            foreach (Session sesh in sessionsOnDate)
            {
                Options.Add(new List<string> {sesh.Date + sesh.Time + ReservationLogic.GetAttractionNameByAttractionID(sesh.AttractionID)});
            }
            MainMenu Menu = new MainMenu(Options, Prompt);
            UiHelpers.Pause();
            int[] selectedIndex = Menu.Run();
            
            Console.ResetColor();

            return selectedIndex[0];
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

                // bool another = UiHelpers.ChoiceHelper("Do you want to book another session?", "Yes, continue.", "No, stop booking.");
                // if (!another) 
                break;
            }


            bool confirm = UiHelpers.ChoiceHelper("Do you want to confirm your reservation?");
        if (confirm)
        {
            string orderNumber = ReservationLogic.GenerateOrderNumber(_customerInfo);
            double totalPrice = 0; // for discount calculation

            foreach (int age in bookingSelections[selectedSession.Id])
            {
                Console.WriteLine("part1");
                double singlePrice = ReservationLogic.CreateSingleTicketBooking(selectedSession.Id, age, _customerInfo, orderNumber, bookingQuantity);
                totalPrice += singlePrice;
            }

            ShowBookingDetails(orderNumber, bookingSelections, totalPrice);

            bool payment = UiHelpers.ChoiceHelper("Proceed to payment?");
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


        public static void ShowBookingDetails(string orderNumber, Dictionary<int, List<int>> bookingDetails, double totalPrice)
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
        DateTime currentDate = DateTime.Now;
        week = System.Globalization.ISOWeek.GetWeekOfYear(currentDate);; 
    }

}