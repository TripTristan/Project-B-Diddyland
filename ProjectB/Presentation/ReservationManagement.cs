using System.Security.Cryptography.X509Certificates;

public class ReservationManagement : AdminElements
{
    public void Run()
    {
        Console.Clear();
        DateTime date = DateTime.Now();

        UiHelpers.WriteHeader("Diddyland – Reservation Management Dashboard");
        string Prompt = $"Logged in as: {_loginStatus.CurrentUserInfo.Username}";

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"View users reservations"},
            new List<string> {"Alter date information"},
            new List<string> {"Quit"},
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();

        switch (selectedIndex[0])
        {
            case 0:
                UserModel user = ViewUsers();
                UserInformationModification(user);
                break;
            case 1:
                date = ReservationUI.DatePicker();
                AlterDateInfo(date);
                break;
            case 2:
                Environment.Exit(0);
                return;
            default:
                break;
        }

    }
    public void AlterDateInfo(DateTime date)
    {
        UiHelpers.WriteHeader("Diddyland – Reservation Management Dashboard");
        string Prompt = $"Logged in as: {_loginStatus.CurrentUserInfo.Username}";

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Change Capacity"},
            new List<string> {"Change VIP capacity"}, 
            new List<string> {"Disable a Timeslot"},
            new List<string> {"Quit"}
        };

        MainMenu Menu = new MainMenu(Options, Prompt);
        int[] selectedIndex = Menu.Run();

        switch (selectedIndex[0])
        {
            case 0:
                ChangeCapacity(false, Date);
                break;
            case 1:
                ChangeCapacity(true, Date);
                break;
            case 3:
                Environment.Exit(0);
                return;
            default:
                break;
        }
    }


    public void ChangeCapacity(DateTime Date, bool VIP)
    {
        SessionModel session = ReservationUI.ShowTimeslotsByDate(_reservationLogic.GetSessionsByDate(Date));
        Console.WriteLine($"Current Capacity: {session.Capacity}");

        ReservationManagementLogic.CapacityChange(session, newCapacity);
    }


}