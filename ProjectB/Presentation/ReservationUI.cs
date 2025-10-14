using System.ComponentModel;

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
            bool guest = LoginOrNot();
            if (!guest) // true is as guest, false is to login
            {
                LoginCustomer();
            }
        }

        List<Session> sessions = _logic.GetAvailableSessions();
        if (sessions.Count == 0)
        {
            Console.WriteLine("Sorry, no available sessions at the moment.");
            return;
        }

        DisplaySessions(sessions);//Show available sessions
        SelectAndProcessSession(sessions);
    }

    public static void LoginCustomer()
    {
        UserLoginUI.StartLogin(); // 
    }

    public static bool LoginOrNot()
    {
        while (true)
        {
            Console.Write("You are not logged in. Continue as guest? (y/n): ");
            string choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y")
            {
                Console.WriteLine("Continuing as guest...");
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
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < sessions.Count)
            {
                return choice;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    public static int GetBookingQuantity(List<Session> sessions, int choice)
    {
        int quantity;
        while (true)
        {
            Console.Write("Enter the number of bookings you want to make: ");
            if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
            {
                var selected = sessions[choice];
                int newQuantity = selected.CurrentBookings + quantity;

                if (newQuantity <= selected.MaxCapacity)
                {
                    return quantity;
                }
                else
                {
                    Console.WriteLine("Not enough available seats. Please choose another quantity.");
                }
            }
        }
    }

    public void SelectAndProcessSession(List<Session> sessions)
    {
        Dictionary<int, int> bookProcessPrepare = new Dictionary<int, int>();


        while (true)
        {
            int choice = GetSessionChoice(sessions);
            int bookingQuantity = GetBookingQuantity(sessions, choice);
            bookProcessPrepare[choice] = bookingQuantity;

            if (AnOtherSession())
            {
                continue;
            }
            else
            {
                if (ConfirmReservation())
                {
                    foreach (var (key, value) in bookProcessPrepare)
                    {
                        string orderNumber = _logic.CreateBooking(sessions[key].Id, value, _customerInfo);
                        ShowSuccessMessage(orderNumber);
                        ShowBookingDetails(orderNumber, bookProcessPrepare);
                    }
                    return;
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
            if (choice == "y") return true;
            if (choice == "n") return false;
            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        }
    }

    public static bool ConfirmReservation()
    {
        while (true)
        {
            Console.Write("Are you sure you want to make this reservation? (y/n): ");
            string choice = Console.ReadLine()?.Trim().ToLower();
            if (choice == "y") return true;
            if (choice == "n") return false;
            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
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
        }
        Console.WriteLine("---------------------");
    }

    public static void ShowSuccessMessage(string orderNumber)
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
        Console.WriteLine($"Order Number: {orderNumber}");
    }
}