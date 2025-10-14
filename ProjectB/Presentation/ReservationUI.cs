public class ReservationUI
{
    private readonly ReservasionLogic _logic;
    private readonly Usermodel _customerInfo;

    public ReservationUI(ReservasionLogic logic, UserModel customerInfo)// customerInfo can be non
    {
        _logic      = logic;
        // _customerInfo = _logic.GetCurrentUserInfo();
    }

    public void StartReservation()
    {
        Console.WriteLine("=== Reservation ===");

        List<SessionDto> sessions = _logic.GetAvailableSessions();
        if (sessions.Count == 0)
        {
            Console.WriteLine("Sorry, no available sessions at the moment.");
            return;
        }
        else
        {
            DisplaySessions(sessions);
            SelectAndProcessSession(sessions);
        }
    }

    public static void DisplaySessions(List<Session> sessions)
    {
        Console.WriteLine("Available Sessions:");
        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            Console.WriteLine($"{i}. Date: {session.Date}, Time: {session.Time}, Available Spots: {session.MaxCapacity - session.CurrentBookings}");
        }
    }

    public static int GetSessionChoice(List<Session> sessions)
    {
        int choice;
        while (true)
        {
            Console.Write("Please select a session number: ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= sessions.Count)
            {
                return choice;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    public static int GetBookingQuantity( List<Session> sessions, int choice)
    {
        int quantity;
        while (true)
        {
            Console.Write("Enter the number of bookings you want to make: ");
            if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
            {
                var selected = sessions[choice];
                int newquantity = selected.CurrentBookings + quantity;

                if (selected.MaxCapacity > newquantity)
                {
                    return quantity;
                }
                else
                {
                    Console.WriteLine("Selected session is fully or can not so much booked.\n Please choose another session or quantity.");
                }
            }
        }
    }

    public static void SelectAndProcessSession(List<Session> sessions)
    {
        Dictionary<int, int> bookProcessPrepare = new Dictionary<int, int>();
        
        while (true)
        {
            int choice = GetSessionChoice(sessions);
            int bookingquantity = GetBookingQuantity(sessions, choice);
            bookProcessPrepare.Add(choice, bookingquantity);

            if (AnOtherSession())
            {
                continue;
            }
            else
            {
                if (ConfirmReservation)
                {
                    foreach (var (key, value) in bookProcessPrepare)
                    {
                        // Get a unike order number for each booking
                        // string orderNumber = _logic.CreateBooking(sessions[key].Id, sessions
                        ShowSuccessMessage(orderNumber);
                        ShowBookingDetails(orderNumber, bookProcessPrepare);
                    }
                }
                else
                {
                    Console.WriteLine("Reservation cancelled.");
                    return;
                }
            }
        }
    }

    public static bool AnOtherSession()
    {
        while (true)
        {
            Console.Write("Do you want to book another session? (y/n): ");
            string choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y")
            {
                return true;
            }
            else if (choice == "n")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            }
        }
    }

    public static bool ConfirmReservation()
    {
        while (true)
        {
            Console.Write("Are you sure you want to make this reservation? (y/n): ");
            string choice = Console.ReadLine()?.Trim().ToLower();
            if (choice == "y")
            {
                return true;
            }
            else if (choice == "n")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            }
        }
    }

    public static void ShowBookingDetails(string orderNumber, Dictionary<int, int> bookingDetails)
    {

        Console.WriteLine("---------------------");
        Console.WriteLine($"Order Number: {orderNumber}");

        Console.WriteLine("Booking Details:");
        foreach (var (key, value) in bookingDetails)
        {
            Console.WriteLine($"Session {key}: {value} bookings");
            // bookingDate
        }
        Console.WriteLine("---------------------");
    }

    public static void ShowSuccessMessage(string orderNumber)
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
        Console.WriteLine($"Order Number: {orderNumber}");
    }
    
}