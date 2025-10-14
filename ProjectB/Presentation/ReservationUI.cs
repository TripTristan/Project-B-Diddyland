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
        var groupedByDate = sessions
            .GroupBy(s => s.Date)
            .ToList();

        if (sessions.Count == 0)
        {
            Console.WriteLine("Sorry, no available sessions at the moment.");
            return;
        }

        Console.WriteLine("Available Sessions:");
        for (int i = 0; i < groupedByDate.Count; i++)
        { // Time in here is only 1 or 2 (morning or afternoon)
        //     var session = sessions[i];
        //     Console.WriteLine($"{i}. Date: {session.Date}, Time: {session.Time}, Available Spots: {session.MaxCapacity - session.CurrentBookings}");
        Console.WriteLine($"{i}. {groupedByDate[i].Key:yyyy-MM-dd}");
        }
    }


    public static int GetSessionChoice()
    {
        int choice;
        while (true)
        {
            Console.Write("Please select a session number: 1 or 2: \n 1. Morning \n 2. Afternoon\n");
            if (int.TryParse(Console.ReadLine(), out choice) && choice == 1 || choice == 2)
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
                if (_logic.CanBookSession(sessions[choice].Id, quantity))
                {
                    return quantity;
                }
                else
                {
                    Console.WriteLine("Not enough available seats.\nPlease choose another quantity.");
                }
            }
        }
    }


    public static int GetDateChoice(List<IGrouping<DateOnly, Session>> groupedByDate)
    {
        int datachoice;
        while (true)
        {
            Console.Write("Please select a date number: ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < groupedByDate.Count)
            {
                return choice;
            }
            Console.WriteLine("Invalid input. Please try again.");
        }
    }


    public static ShowSessionsByDate(List<IGrouping<DateOnly, Session>> groupedByDate, int dateChoice)
    {
        Console.WriteLine($"Available Sessions on {groupedByDate[dateChoice].Key:yyyy-MM-dd}:");
        var sessionsOnDate = groupedByDate[dateChoice].ToList();
        for (int j = 0; j < sessionsOnDate.Count; j++)
        {
            var session = sessionsOnDate[j];
            Console.WriteLine($"{j}. Time: {session.Time}, Available Spots: {session.MaxCapacity - session.CurrentBookings}");
        }
    }


    //#########################################################################################################################################
    public void SelectAndProcessSession(List<Session> sessions)
    {
        Dictionary<int, int, int> bookProcessPrepare = new Dictionary<int, int>();


        while (true)
        {
            var groupedByDate = sessions
            .GroupBy(s => s.Date)
            .ToList();


            int dateChoice = GetDateChoice(groupedByDate);
            ShowSessionsByDate(groupedByDate, dateChoice);
            int sessionChoice = GetSessionChoice();//1 or 2  morning or afternoon
            



            int bookingQuantity = GetBookingQuantity(sessionsChoice, choice);

            bookProcessPrepare[dateChoice][sessionChoice] = bookingQuantity;

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
                        // default here



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
    

    public static bool DateSelect()
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