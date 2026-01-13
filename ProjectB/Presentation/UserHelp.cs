using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

public class UserHelp
{
    private UserContext _ctx;
    public UserHelp(UserContext a) { _ctx = a; }

    public void Run()
    {
        Console.Clear();

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Complaint about food"},
            new List<string> {"Complaint about staff or service"},
            new List<string> {"Complaint about safety"},
            new List<string> {"Complaint about organization"}
        };
        MainMenu Menu = new MainMenu(Options, "Which complaint do you have?");
        int[] selectedIndex = Menu.Run();

        Console.Clear();


        switch (selectedIndex[0])
        {
            case 0:
                Complain("quality@diddyland.com");
                break;
            case 1:
                Complain(" hr@diddyland.com");
                break;
            case 2:
                Complain("safetycoach@diddyland.com");
                break;
            case 3:
                Complain("bob.bob@diddyland.com");
                break;
            default:
                break;
        }


        Complain(selectedIndex);

        Console.WriteLine("\n✅ Your complaint has been saved. Thank you!");
        Console.WriteLine("We appreciate your feedback and will work to improve.\n");

    }

    private void Complain(int[] selectedIndex)
    {
        string[] locations =
        {
            "DiddyLand - Amsterdam",
            "DiddyLand - Rotterdam"
        };

        Console.WriteLine("\nSelect park location:");
        for (int i = 0; i < locations.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {locations[i]}");
        }

        Console.Write("\nEnter location number: ");
        string locInput = Console.ReadLine();
        int locChoice = int.TryParse(locInput, out int locNum) ? locNum : 1;
        if (locChoice < 1 || locChoice > locations.Length) locChoice = 1;

        string location = locations[locChoice - 1];
        Console.WriteLine("\nPlease describe your complaint below:");
        string description = Console.ReadLine();

        string username = _ctx.loginStatus.CurrentUserInfo?.Username ?? "Anonymous";
        string category = Options[selectedIndex[0]][0];

        _ctx.complaintLogic.SubmitComplaint(username, category, description, location);
    }

    public static void Complain(string mail)
    {
        Console.WriteLine($@"For any small complaints, write here.
    For larger complaints, you can contact us via:
    • E-mail: {mail}
    • SMS: +31 0181 982513
    • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public void ShowHandledMessages()
    {
        string? username = _ctx.loginStatus.CurrentUserInfo?.Username;


        ShowPendingMessages();

        var handledComplaints = _ctx.complaintLogic.GetByUserAndStatus(username, "Handled");

        if (!handledComplaints.Any())
            return;

        Console.WriteLine("You have some complaints that have been handled:\n");
        foreach (var c in handledComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("This complaint has been handled by our staff.\n");
        }
    }

    public void ShowPendingMessages()
    {
        string username = _ctx.loginStatus.CurrentUserInfo?.Username ?? "Anonymous";
        var pendingComplaints = _ctx.complaintLogic.GetPendingByUser(username);

        if (!pendingComplaints.Any())
            return;

        Console.WriteLine("You have pending complaints:\n");
        foreach (var c in pendingComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("This complaint is still pending.\n");
        }
    }


    private void ComplaintFood() { }
    private void ComplaintStaff() { }
    private void ComplaintSafety() { }
    private void ComplaintOrganization() { }
}
