using System;
using System.Collections.Generic;
using System.Linq;

public class InboxUI
{
    private readonly ComplaintLogic _logic;
    private readonly LoginStatus _loginStatus;

    public InboxUI(ComplaintLogic logic, LoginStatus loginStatus)
    {
        _logic = logic;
        _loginStatus = loginStatus;
    }

    public void Show()
    {
        string username = _loginStatus.CurrentUserInfo?.Username ?? "Anonymous";

        if (username == "Guest")
        {
            Console.WriteLine("Guest accounts do not have access to the inbox.");
            UiHelpers.Pause();
            return;
        }

        var complaints = _logic
            .FilterComplaints(username: username)
            .OrderBy(c => c.Status == "Open" ? 0 : 1)
            .ThenByDescending(c => c.CreatedAt)
            .ToList();

        if (!complaints.Any())
        {
            Console.WriteLine("You have no complaints on file.");
            UiHelpers.Pause();
            return;
        }

        Console.WriteLine("\n=== Your Complaints ===\n");

        foreach (var complaint in complaints)
        {
            PrintComplaint(complaint);
        }

        UiHelpers.Pause();
    }

    private void PrintComplaint(ComplaintModel complaint)
    {
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine($"Complaint ID  : {complaint.Id}");
        Console.WriteLine($"Category      : {complaint.Category}");
        Console.WriteLine($"Location      : {complaint.Location}");
        Console.WriteLine($"Submitted     : {complaint.CreatedAt:dd-MM-yyyy HH:mm}");
        Console.WriteLine($"Status        : {complaint.Status}");
        Console.WriteLine($"Description   : {complaint.Description}");

        if (!string.IsNullOrWhiteSpace(complaint.AdminResponse) && complaint.AdminResponse != "-")
        {
            Console.WriteLine($"Admin Response: {complaint.AdminResponse}");
        }

        Console.WriteLine("------------------------------------------------\n");
    }
}
