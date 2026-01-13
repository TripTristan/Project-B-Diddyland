public class AdminFinance
{
    private Dependencies _ctx;
    public AdminFinance(Dependencies a) { _ctx = a; }


    public void Run()
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
    public void ShowRevenueByDate()
    {
        DateTime firstDate = new();
        DateTime secondDate = new();

        for (int i = 0; i != 2; i++)
        {
            int month = DateSelection.monthMenu();
            List<List<string>> Options = DateSelection.DaysInSelectedMonth(month);
            MainMenu DayChoice = new MainMenu(Options, "Select the date:");
            if (i == 0)
            {
                firstDate = DateSelection.GetDateFromCoordinate(DayChoice.Run(), 2025, month);
            }
            else
            {
                secondDate = DateSelection.GetDateFromCoordinate(DayChoice.Run(), 2025, month);
            }
        }

        if (firstDate > secondDate)
        {
            DateTime temp = firstDate;
            firstDate = secondDate;
            secondDate = temp;
        }

        List<ReservationModel> Orders = _ctx.financialLogic.GetRevenueByDateRange(firstDate.Ticks, secondDate.Ticks);
        InformationFormatUser(Orders, $"{firstDate.ToString("MM/dd/yyyy")} - {secondDate.ToString("MM/dd/yyyy")}");

    }

    public void ShowRevenueByPerson()
    {
        List<UserModel> users = _ctx.financialLogic.GrabAllUsers();

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

        List<ReservationModel> userOrders = _ctx.financialLogic.GetAllUserOrders(selectedUser);
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
            UiHelpers.Good($" ${order.Price}");
            total += order.Price;
        }

        Console.WriteLine("-------------------------");
        UiHelpers.Good($"Total: ${total}");
        UiHelpers.Pause();
    }
}