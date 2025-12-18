public class AdminContext
{
    public LoginStatus loginStatus { get; }
    public UserAuthentication userAuth { get; }

    public AttractionLogic attractionLogic { get; }
    public FoodmenuLogic foodmenuLogic { get; }
    public FinancialLogic financialLogic { get; }
    public ComplaintLogic complaintLogic { get; }
    public DiscountLogic discountLogic { get; }
    public UserLogic userLogic { get; }

    public AdminContext(
        LoginStatus loginStatus,
        UserAuthentication userAuth,
        AttractionLogic attractionLogic,
        FoodmenuLogic foodmenuLogic,
        FinancialLogic financialLogic,
        ComplaintLogic complaintLogic,
        DiscountLogic discountLogic,
        UserLogic userLogic)
    {
        this.loginStatus = loginStatus;
        this.userAuth = userAuth;
        this.attractionLogic = attractionLogic;
        this.foodmenuLogic = foodmenuLogic;
        this.financialLogic = financialLogic;
        this.complaintLogic = complaintLogic;
        this.discountLogic = discountLogic;
        this.userLogic = userLogic;
    }

    public UserModel SelectUser()
    {
        List<UserModel> users = financialLogic.GrabAllUsers();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            UiHelpers.Pause();
            return null;
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

        return users[selected[0]];
    }

}
