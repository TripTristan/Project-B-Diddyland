public class FinancialMenu
{
    public static List<string> Months = new() {"January", "February", "  March  ", " April ", "   May"  , "   June  ", " July  ", " August ", "September", "October", "November", "December "};
    
    private readonly FinancialLogic _financialLogic;

    public FinancialMenu(FinancialLogic financialLogic)
    {
        _financialLogic = financialLogic;
    }

    public void Start()
    {
        string Prompt = "Select the category you want to view";

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Users"},
            // new List<string> {"Product"}, 
            new List<string> {"Date"}, 
            new List<string> {"All"}
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        switch (selectedIndex[0])
        {
            case 0:
                ShowRevenueByPerson();
                break;
            // case 1:
            //     ShowRevenueByProduct();
            //     break;
            case 1:
                ShowRevenueByDate();
                break;
            default:
                Console.WriteLine("");
                break;
        }
    }

    public static int monthMenu()
    {
        List<List<string>> Months = new List<List<string>>
        {
            new List<string> { "January", "February", "  March  " },
            new List<string> { " April ", "   May"  , "   June  " },
            new List<string> { " July  ", " August ", "September" },
            new List<string> { "October", "November", "December " }
        };

        List<List<int>> MonthsNRS = new List<List<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5, 6 },
            new List<int> { 7, 8 , 9 },
            new List<int> { 10, 11, 12 }
        };

        MainMenu monthMenu = new MainMenu(Months, "Select the month you want to view");
        int[] selectedMonth = monthMenu.Run();
        int month = MonthsNRS[selectedMonth[0]][selectedMonth[1]];
        return month;
        
    }

    public static List<List<string>> DaysInSelectedMonth(int month)
    {
        List<List<string>> Options = new();

        int year = 2025;
        int daysInMonth = DateTime.DaysInMonth(year, month);
        List<string> currentWeek = new List<string>();

        for (int day = 1; day <= daysInMonth; day++)
        {
            if (day < 10) currentWeek.Add(" " + day.ToString());
            else currentWeek.Add(day.ToString());

            if (currentWeek.Count == 7)
            {
                Options.Add(currentWeek);
                currentWeek = new List<string>();
            }
        }
        if (currentWeek.Count > 0) Options.Add(currentWeek);

        return Options;
    }
    public void ShowRevenueByDate()
    {
        DateTime firstDate = new();
        DateTime secondDate = new();
        

        for (int i = 0; i != 2; i++)
        {
            int month = monthMenu();
            List<List<string>> Options = DaysInSelectedMonth(month);
            MainMenu DayChoice = new MainMenu(Options, "Select the date:");
            if (i == 0)
            {
                firstDate = _financialLogic.GetDateFromCoordinate(DayChoice.Run(), 2025, month);
            }
            else
            {
                secondDate = _financialLogic.GetDateFromCoordinate(DayChoice.Run(), 2025, month);
            }
        }

        if (firstDate > secondDate)
        {
            DateTime temp = firstDate;
            firstDate = secondDate;
            secondDate = temp;
        }

        List<ReservationModel> Orders = _financialLogic.GetRevenueByDateRange(firstDate.Ticks, secondDate.Ticks);
        InformationFormatUser(Orders, $"{firstDate.ToString("MM/dd/yyyy")} - {secondDate.ToString("MM/dd/yyyy")}");
        

    }

    public void ShowRevenueByPerson()
    {
        List<UserModel> users = _financialLogic.GrabAllUsers();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            UiHelpers.Pause();
            return;
        }

        List<List<string>> Options = new List<List<string>>();

        for (int i = 0; i < users.Count; i++)
        {
            List<string> temp = new List<string>();
            temp.Add(users[i].Name);
            Options.Add(temp);
        }

        MainMenu menu = new MainMenu(Options, "Select a user to view revenue:");
        int[] selected = menu.Run();

        UserModel selectedUser = users[selected[0]];

        List<ReservationModel> userOrders = _financialLogic.GetAllUserOrders(selectedUser);
        
        Console.Clear();
        InformationFormatUser(userOrders, selectedUser.Name);
    }

    // the database currently doesnt register whether a product is bought or not, and its quantity, so imma leasve this for later

    // public void ShowRevenueByProduct()
    // {
        
    //     InformationFormatProduct("Revenue for " + selectedProduct.Name, revenues);
    // }

    public void InformationFormatUser(List<ReservationModel> userOrders, string name)
    {
        Console.WriteLine("---- Revenue for " + name + " ----");
        double total = 0;

        foreach (ReservationModel order in userOrders)
        {
            Console.Write(new DateTime(order.BookingDate).ToString("d") + ":");
            UiHelpers.Good($" ${order.Price/100}");
            total += order.Price/100;
        }

        Console.WriteLine("-------------------------");
        UiHelpers.Good($"Total: ${total}");
        UiHelpers.Pause();
    }
}

        