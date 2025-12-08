    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;

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

    
    // public static int week = Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    public static int week = 0;
    public static void StartReservation()
    {
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
    

    private void DisplayDates(List<Session> sessions)
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

        int seshId = 1;

        foreach (var group in orderedGroups)
        {
            var orderedInDay = group
                .Where(s => !IsFastPassSlot(s.Time))
                .GroupBy(s => s.Time.Trim().ToLower())
                .Select(g => g.First())
                .OrderBy(s => s.Time);

            Console.WriteLine($"\nDate: {group.Key:yyyy-MM-dd}\nTime Slots:");
            
            Console.WriteLine();
        }
        return $"Select a week";
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
            MainMenu Menu = new MainMenu(Options, Prompt);
            UiHelpers.Pause();
            int[] selectedIndex = Menu.Run();
            
            Console.ResetColor();

            return selectedIndex[0];
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

            bookingSelections[selectedSession.Id] = ages;

                // bool another = UiHelpers.ChoiceHelper("Do you want to book another session?", "Yes, continue.", "No, stop booking.");
                // if (!another) 
                break;
            }


        confirm = UiHelpers.ChoiceHelper("Do you want to confirm your reservation?");
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
            Console.WriteLine("Payment cancelled. Reservation NOT confirmed.");
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

    private void ShowBookingDetails(string orderNumber, Dictionary<int, List<int>> bookingDetails, decimal totalPrice)
    {
        DateTime currentDate = DateTime.Now;
        week = System.Globalization.ISOWeek.GetWeekOfYear(currentDate);; 
    }




NEED REWORKK