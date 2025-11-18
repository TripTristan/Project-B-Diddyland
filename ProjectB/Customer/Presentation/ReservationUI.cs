public class ReservationUI
{
    private readonly IReservationLogic _logic;
    private readonly ILoginLogic _loginLogic;
    private readonly IPaymentService _paymentService;
    private UserModel? _currentUser;

    public ReservationUI(IReservationLogic logic, ILoginLogic loginLogic, IPaymentService paymentService)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _loginLogic = loginLogic ?? throw new ArgumentNullException(nameof(loginLogic));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _currentUser = null;
    }

    public void StartReservation(UserModel? CurrentUser = null)
    {

        Console.Clear();
        Console.WriteLine("=== Reservation System ===\n");

        _currentUser = CurrentUser;


        if (_currentUser == null)
        {
            bool continueAsGuest = PromptForChoice(
                "You are not logged in. Continue as guest?",
                "Yes, continue as guest.",
                "No, I want to log in.");

            if (!continueAsGuest)
            {
                _currentUser = PerformLogin();
                if (_currentUser == null)
                {
                    Console.WriteLine("Login failed or cancelled. Reservation aborted.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Continuing as guest.");
            }
        }

        var sessions = _logic.GetAvailableSessions();
        if (sessions.Count == 0)
        {
            Console.WriteLine("No available sessions.");
            return;
        }

        var groupedByDate = sessions.GroupBy(s => s.Date.Date).OrderBy(g => g.Key).ToList();

        var bookingSelections = CollectBookingSelections(groupedByDate);

        if (!bookingSelections.Any())
        {
            Console.WriteLine("No sessions selected. Reservation cancelled.");
            return;
        }

        var orderNumber = _logic.GenerateOrderNumber(_currentUser);
        var (totalFinalPrice, agePricing, discountDescriptions) = CalculateBookingDetails(
            bookingSelections, groupedByDate, orderNumber);

        ShowBookingDetails(orderNumber, bookingSelections, groupedByDate, agePricing, discountDescriptions, totalFinalPrice);

        if (!PromptForChoice("Do you want to confirm your reservation?", "Yes, confirm.", "No, cancel."))
        {
            Console.WriteLine("Reservation cancelled.");
            return;
        }


        if (PromptForChoice("Proceed to payment?", "Yes, proceed.", "No, cancel."))
        {
            PaymentUI.StartPayment(
                orderNumber,
                bookingSelections,
                groupedByDate,
                discountDescriptions,
                totalFinalPrice,
                _currentUser ?? new UserModel { userNr = "1", Name = "Guest", PhoneNr = "",  Email = "", DateofBirth = "", Address = "", Account = "", Password = "" },
                _paymentService
            );

            Console.WriteLine("Reservation process complete.");
        }
        else
        {
            Console.WriteLine("Payment cancelled. Reservation not confirmed.");
        }
    }

    private UserModel? PerformLogin()
    {
        var loginUI = new UserLoginUI(_loginLogic);
        return loginUI.StartLogin();
    }

    private Dictionary<int, Dictionary<int, List<int>>> CollectBookingSelections(
        List<IGrouping<DateTime, Session>> groupedByDate)
    {
        var selections = new Dictionary<int, Dictionary<int, List<int>>>();

        while (true)
        {

            Console.WriteLine("\nAvailable Dates and Sessions:");
            for (int gi = 0; gi < groupedByDate.Count; gi++)
            {
                Console.WriteLine($"\n[{gi}] Date: {groupedByDate[gi].Key:yyyy-MM-dd}");
                var list = groupedByDate[gi].OrderBy(s => s.Time).ToList();
                for (int si = 0; si < list.Count; si++)
                {
                    var s = list[si];
                    Console.WriteLine($"   {si}. {s.Time:hh\\:mm} - {s.Name} (ID: {s.Id}) | Available: {s.MaxCapacity - s.CurrentBookings}");
                }
            }

            int dateChoice = PromptForInt("\nSelect date group number (or -1 to finish): ", -1, groupedByDate.Count - 1);
            if (dateChoice == -1) break;

            var sessionsOnDate = groupedByDate[dateChoice].OrderBy(s => s.Time).ToList();
            if (sessionsOnDate.Count == 0)
            {
                Console.WriteLine("No sessions on that date. Try again.");
                continue;
            }

            int sessionChoice = PromptForInt("Select session number: ", 0, sessionsOnDate.Count - 1);
            var selectedSession = sessionsOnDate[sessionChoice];

            int bookingQuantity = GetBookingQuantity(selectedSession);
            var ages = CollectAges(bookingQuantity);

            if (!selections.ContainsKey(dateChoice))
                selections[dateChoice] = new Dictionary<int, List<int>>();

            if (selections[dateChoice].ContainsKey(selectedSession.Id)) // if already booked this session, append one ages[]
                selections[dateChoice][selectedSession.Id].AddRange(ages);
            else
                selections[dateChoice][selectedSession.Id] = new List<int>(ages);

            if (!PromptForChoice("Book another session?", "Yes, continue.", "No, stop booking."))
                break;
        }

        return selections;
    }

    private List<int> CollectAges(int count)
    {
        var ages = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int age = PromptForInt($"Enter age for ticket {i + 1}: ", 0, 120);
            ages.Add(age);
        }
        return ages;
    }

    private (decimal totalFinalPrice,
            Dictionary<int, List<(decimal discount, decimal finalPrice)>> agePricing,
            Dictionary<string, decimal> discountDescriptions)
        CalculateBookingDetails(
            Dictionary<int, Dictionary<int, List<int>>> bookingSelections,
            List<IGrouping<DateTime, Session>> groupedByDate,
            string orderNumber)
    {
        var agePricing = new Dictionary<int, List<(decimal discount, decimal finalPrice)>>();
        decimal subtotal = 0m;
        int totalTickets = 0;

        foreach (var (groupIdx, inner) in bookingSelections)
        {
            foreach (var (sessionId, ages) in inner)
            {
                if (!agePricing.ContainsKey(sessionId))
                    agePricing[sessionId] = new List<(decimal, decimal)>();

                foreach (int age in ages)
                {
                    var (discount, basePrice, finalPrice) = _logic.CreateSingleTicketBooking(
                        sessionId, age, _currentUser, orderNumber, totalTickets);

                    agePricing[sessionId].Add((discount, finalPrice));
                    subtotal += finalPrice;
                    totalTickets++;
                }
            }
        }

        var (discountDescriptions, offerDiscount, totalFinalPrice) = _logic.CalculateOfferDiscountedPrice(totalTickets, subtotal, _currentUser);

        return (totalFinalPrice, agePricing, discountDescriptions);
    }

    private int GetBookingQuantity(Session session)
    {
        while (true)
        {
            int quantity = PromptForInt("Enter booking quantity: ", 1, 10);
            if (_logic.CanBookSession(session.Id, quantity))
                return quantity;

            Console.WriteLine("Not enough seats available. Please try again.");
        }
    }

    private static bool PromptForChoice(string message, string yesOption, string noOption)
    {
        while (true)
        {
            Console.WriteLine($"\n{message}");
            Console.WriteLine($" y - {yesOption}");
            Console.WriteLine($" n - {noOption}");
            Console.Write("Your choice (y/n): ");

            var choice = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (choice == "y") return true;
            if (choice == "n") return false;

            Console.WriteLine("Invalid input. Enter 'y' or 'n'.");
        }
    }

    private static int PromptForInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Invalid input. Enter a number between {min} and {max}.");
        }
    }

    private static void ShowBookingDetails(
        string orderNumber,
        Dictionary<int, Dictionary<int, List<int>>> bookingSelections,
        List<IGrouping<DateTime, Session>> groupedByDate,
        Dictionary<int, List<(decimal discount, decimal finalPrice)>> agePricing,
        Dictionary<string, decimal> discountDescriptions,
        decimal totalFinalPrice)
    {
        Console.WriteLine("\n===================== BOOKING SUMMARY =====================");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine("-----------------------------------------------------------");

        foreach (var (groupIndex, sessionBookings) in bookingSelections)
        {
            var actualDate = groupedByDate[groupIndex].Key;
            Console.WriteLine($"\nDate: {actualDate:dddd, MMMM dd, yyyy}");

            foreach (var (sessionId, ages) in sessionBookings)
            {
                int childCount = ages.Count(age => age < 12);
                int seniorCount = ages.Count(age => age >= 65);
                int adultCount = ages.Count(age => age >= 12 && age < 65);

                Console.WriteLine($"\n  Session ID: {sessionId}");
                if (childCount > 0) Console.WriteLine($"    Child (0-11): {childCount}");
                if (adultCount > 0) Console.WriteLine($"    Adult (12-64): {adultCount}");
                if (seniorCount > 0) Console.WriteLine($"    Senior (65+): {seniorCount}");

                if (agePricing.TryGetValue(sessionId, out var prices))
                {
                    Console.WriteLine($"    Subtotal: {prices.Sum(p => p.finalPrice):C}");
                }
            }
        }

        if (discountDescriptions?.Any() == true)
        {
            Console.WriteLine("\n-----------------------------------------------------------");
            Console.WriteLine("Applied Discounts:");
            foreach (var (desc, amount) in discountDescriptions)
            {
                Console.WriteLine($"  {desc}: -{amount:C}");
            }
        }

        Console.WriteLine("\n-----------------------------------------------------------");
        Console.WriteLine($"TOTAL AMOUNT: {totalFinalPrice:C}");
        Console.WriteLine("===========================================================\n");
    }
}