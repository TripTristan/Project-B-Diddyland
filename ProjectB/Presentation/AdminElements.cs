public class AdminElements
{
    private readonly LoginStatus _loginStatus;
    private readonly UiHelpers _ui;
    private readonly AttractieMenu _attractieMenu;
    private readonly MenuForm _menuForm;
    private readonly OrderForm _orderForm;
    private readonly ReservationManagement _reservationManagementUI;
    private readonly ParkMap _parkMap;
    private readonly ManageAdmins _manageAdmins;
    private readonly AdminComplaintsPage _adminComplaintsPage;
    private readonly UserLogoutUI _logoutUi;
    private readonly FinancialMenu _financialMenu;
    private readonly DiscountCodeUI _discountCodeUI;

    public AdminElements(
        LoginStatus loginStatus,
        UiHelpers ui,
        AttractieMenu attractieMenu,
        MenuForm menuForm,
        OrderForm orderForm,
        ReservationUI reservationUI,
        ParkMap parkMap,
        ManageAdmins manageAdmins,
        AdminComplaintsPage adminComplaintsPage,
        UserLogoutUI logoutUi,
        FinancialMenu financialMenu,
        DiscountCodeUI discountCodeUI)
    {
        _loginStatus = loginStatus;
        _ui = ui;
        _attractieMenu = attractieMenu;
        _menuForm = menuForm;
        _orderForm = orderForm;
        _reservationUI = reservationUI;
        _parkMap = parkMap;
        _manageAdmins = manageAdmins;
        _adminComplaintsPage = adminComplaintsPage;
        _logoutUi = logoutUi;
        _financialMenu = financialMenu;
        _discountCodeUI = discountCodeUI;
    }

    public UserModel GetUsers()
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
    }
}