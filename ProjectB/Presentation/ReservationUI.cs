    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;

public class ReservationUI
{
    public static List<string> AgeOptions = new() {"0-15 : ", "16-60 : ","61+   : ",};
    public static List<string> TimeslotOptions = new() {"09:00-13:00", "13:00-17:00", "17:00-21:00"};
    private readonly ReservationLogic _reservationLogic;
    private readonly PaymentUI _paymentUI;
    private readonly UserLoginUI _loginUI;
    private readonly UiHelpers _ui;
    private readonly SessionAccess _sessionAccess;
    private readonly LoginStatus _loginStatus;
    private readonly FinancialLogic _financialLogic;
    private readonly DiscountCodeLogic _discountLogic;

    private UserModel? _customerInfo;
    private int _week = 0;

    public ReservationUI(
        ReservationLogic reservationLogic,
        PaymentUI paymentUI,
        UserLoginUI loginUI,
        UiHelpers ui,
        SessionAccess sessionAccess,
        LoginStatus loginStatus,
        FinancialLogic financialLogic,
        DiscountCodeLogic discountLogic)
    {
        _reservationLogic = reservationLogic;
        _paymentUI = paymentUI;
        _loginUI = loginUI;
        _ui = ui;
        _sessionAccess = sessionAccess;
        _loginStatus = loginStatus;
        _financialLogic = financialLogic;
        _discountLogic = discountLogic;
    }

    
    // public static int week = Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    public static int week = 0;
    public void StartReservation()
    {
        DateTime ChosenDate = DatePicker();
        SessionModel session = ShowTimeslotsByDate(_reservationLogic.GetSessionsByDate(ChosenDate));
        List<int> GuestsAges = GuestQuantitySelection();
        double Price = _reservationLogic.CalculatePriceForGuests(GuestsAges);

        Console.Write("\nDo you have a discount code? (enter or leave blank): ");
        string? code = Console.ReadLine()?.Trim();

        double finalPrice = _discountLogic.Apply(code, Price);

        Console.WriteLine($"\nFinal Price (after discount if any): {finalPrice:C}");

        ShowBookingDetails(ChosenDate.Ticks, _reservationLogic.GenerateOrderNumber(_loginStatus.CurrentUserInfo), session, GuestsAges, Price);
        if (UiHelpers.ChoiceHelper("Confirm order"))
        {
            _reservationLogic.CreateSingleTicketBooking(session.Id, (GuestsAges[0] + GuestsAges[1] + GuestsAges[2]), _loginStatus.CurrentUserInfo, Price);
        }
        ShowSuccessMessage();
        UiHelpers.Pause();

    }

    private static void ShowBookingDetails(long chosenDate, string orderNumber, SessionModel session, List<int> GuestsAges, double totalPrice)
    {
        Console.Clear();
        UiHelpers.WriteHeader("Booking Details:\n");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine($"Date: {chosenDate.ToString("dd-MM-yyyy")} {TimeslotOptions[(int)session.Time]}\n");


        for (int i = 0; i<3; i++)
        {
            Console.WriteLine($"{  AgeOptions[i]  }{GuestsAges[i]}");
        }


        Console.WriteLine($"\nTotal Price: {totalPrice:C}");
    }


    private DateTime DatePicker()
    {
        DateTime DateSelected = DateTime.Now;
        

        int month = FinancialMenu.monthMenu();
        MainMenu DayChoice = new MainMenu(FinancialMenu.DaysInSelectedMonth(month), $"           {FinancialMenu.Months[month-1]}         ");
        DateSelected = _financialLogic.GetDateFromCoordinate(DayChoice.Run(), 2025, month);

        if (DateSelected.Ticks < DateTime.Now.Ticks)
        {
            Console.WriteLine("You can't book into the past");
            UiHelpers.Pause();
            DateSelected = DatePicker();
        }

        if (!_reservationLogic.CheckForTimeslotsOnDate(DateSelected))
            { _reservationLogic.PopulateTimeslots(DateSelected); }
            
        return DateSelected;
    }


    private bool IsFastPassSlot(string time)
    {
        if (string.IsNullOrWhiteSpace(time)) return false;
        return TimeSpan.TryParse(time.Trim().ToLower(), out _);
    }

    private SessionModel ShowTimeslotsByDate(List<SessionModel> sessions)
    {
        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {_reservationLogic.AvailabilityFormatter(sessions[0])},
            new List<string> {_reservationLogic.AvailabilityFormatter(sessions[1])}, 
            new List<string> {_reservationLogic.AvailabilityFormatter(sessions[2])}
        };

        MainMenu Menu = new MainMenu(Options, $"Select A Timeslot for\n {sessions[0].Date.ToString("dd-MM-yyyy")}");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        return sessions[selectedIndex[0]];
    }

    private List<int> GuestQuantitySelection()
    {
        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {$"0-15  : 0"},
            new List<string> {$"16-60 : 0"}, 
            new List<string> {$"61+   : 0"}, 
        };

        MainMenu Menu = new MainMenu(Options, "Years  | Qty");
        List<int> SelectedIndice = Menu.Run(1);
        UiHelpers.Pause();

        return SelectedIndice;
    }



    private static void ShowSuccessMessage()
    {
        Console.WriteLine("Reservation successful! Thank you for booking with us.");
    }

    private void ShowBookingDetails(string orderNumber, Dictionary<int, List<int>> bookingDetails, decimal totalPrice)
    {
        DateTime currentDate = DateTime.Now;
        week = System.Globalization.ISOWeek.GetWeekOfYear(currentDate);; 
    }
}