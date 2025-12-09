using System;
using System.Collections.Generic;
using System.Linq;

public class AdminComplaintsPage
{
    static string status;
    private readonly ComplaintLogic _complaintLogic;
    private readonly UiHelpers _ui;

    public AdminComplaintsPage(ComplaintLogic complaintLogic, UiHelpers ui)
    {
        _complaintLogic = complaintLogic;
        _ui = ui;
    }

    public void Show()
    {
        Console.WriteLine("Select park location:");
        Console.WriteLine("1) Diddyland Rotterdam");
        Console.WriteLine("2) Diddyland Amsterdam");
        string choice = Console.ReadLine() ?? "1";
        string location = choice == "2" ? "DiddyLand - Amsterdam" : "DiddyLand - Rotterdam";

        while (true)
        {
            Console.Clear();
            string Prompt = "Admin – Complaint Management";

            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"View all complaints"},
                new List<string> {"Filter by category"},       
                new List<string> {"Filter by username"},
                new List<string> {"Filter by status"},
                new List<string> {"Mark complaint as handled"},
                new List<string> {"Delete complaint"},
                new List<string> {"Back"}
            };

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    ViewAll(location);
                    break;
                case 1:
                    FilterByCategory(location);
                    break;
                case 2:
                    FilterByUser(location);
                    break;
                case 3:
                    FilterByStatus(location);
                    break;
                case 4:
                    MarkHandled(location);
                    break;
                case 5:
                    DeleteComplaint(location);
                    break;
                case 6:
                    return;
                default:
                    UiHelpers.Warn("Invalid choice.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }

    private void ViewAll(string location)
    {
        var complaints = _complaintLogic.FilterComplaints(location: location);
        Console.Clear();
        UiHelpers.WriteHeader($"All Complaints ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}\n");
        }
        UiHelpers.Pause();
    }

    private void FilterByCategory(string location)
    {
        Console.Clear();
        UiHelpers.WriteHeader("Filter Complaints by Category");

        string[] categories =
        {
            "Complaint about food",
            "Complaint about staff or service",
            "Complaint about safety",
            "Complaint about organization"
        };

        Console.WriteLine("Available categories:");
        for (int i = 0; i < categories.Length; i++)
            Console.WriteLine($"{i + 1}. {categories[i]}");

        Console.Write("\nEnter category number (1–4): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 1 || choice > categories.Length)
        {
            UiHelpers.Warn("Invalid choice. Please enter a number between 1 and 4.");
            UiHelpers.Pause();
            return;
        }

        string category = categories[choice - 1];
        var complaints = _complaintLogic.FilterComplaints(category: category, location: location);

        Console.Clear();
<<<<<<< HEAD
        UiHelpers.WriteHeader($"Complaints in category: {category} ({location})");
=======
        _ui.WriteHeader($"Complaints in category: {category} ({location})");

>>>>>>> f9dc270555a268f44f2ce5154d1282485432fb25
        if (complaints.Count == 0)
        {
            Console.WriteLine("No complaints found for this category.");
        }
        else
        {
            foreach (var c in complaints)
            {
                Console.WriteLine($"[{c.Id}] {c.Username} - {c.Status}");
                Console.WriteLine($"    {c.Description}\n");
            }
        }
        UiHelpers.Pause();
    }

    private void FilterByUser(string location)
    {
        var usernames = _complaintLogic
            .FilterComplaints(location: location)
            .Select(c => c.Username)
            .Distinct()
            .OrderBy(u => u)
            .ToList();

        if (usernames.Count == 0)
        {
            Console.WriteLine("No users found for this location.");
            UiHelpers.Pause();
            return;
        }

        Console.Clear();
        UiHelpers.WriteHeader("Select a Username");

        for (int i = 0; i < usernames.Count; i++)
            Console.WriteLine($"{i + 1}. {usernames[i]}");

        Console.Write("\nEnter number: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > usernames.Count)
        {
            UiHelpers.Warn("Invalid choice.");
            UiHelpers.Pause();
            return;
        }

        string selectedUser = usernames[choice - 1];
        var complaints = _complaintLogic.FilterComplaints(username: selectedUser, location: location);

        Console.Clear();
        UiHelpers.WriteHeader($"Complaints by {selectedUser} ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}\n");
        }
        UiHelpers.Pause();
    }

    private void FilterByStatus(string location)
    {

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Open"},
            new List<string> {"Handled"}
        };

        MainMenu Menu = new MainMenu(Options, "Filter Status");
        int[] selectedIndex = Menu.Run();

        switch (selectedIndex[0])
        {
            case 0:
                status = "Open";
                break;
            case 1:
                status = "Handled";
                break;
        }
        List<ComplaintModel> complaints = _complaintLogic.RetrieveComplaintsWithStatus(status);

        var statuses = _complaintLogic
            .FilterComplaints(location: location)
            .Select(c => c.Status)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        if (statuses.Count == 0)
        {
            Console.WriteLine("No statuses found for this location.");
            UiHelpers.Pause();
            return;
        }


        Console.Clear();
        UiHelpers.WriteHeader("Select a Status");

        for (int i = 0; i < statuses.Count; i++)
            Console.WriteLine($"{i + 1}. {statuses[i]}");

        string selectedStatus = statuses[selectedIndex[0]];
        var complaint = _complaintLogic.FilterComplaints(status: selectedStatus, location: location);

        Console.Clear();
        UiHelpers.WriteHeader($"Complaints with status {selectedStatus} ({location})");

        foreach (var c in complaint)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category}");
            Console.WriteLine($"    {c.Description}\n");
        }
        UiHelpers.Pause();
    }

    private void MarkHandled(string location)
    {
        List<ComplaintModel> openComplaints = _complaintLogic.FilterComplaints(location: location, status: "Open");

        if (openComplaints.Count == 0)
        {
            Console.WriteLine("No open complaints to handle.");
            UiHelpers.Pause();
            return;
        }

        Console.Clear();
        UiHelpers.WriteHeader("Open Complaints");
        foreach (var c in openComplaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}\n");
        }

        Console.Write("Enter complaint ID to mark as handled: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            _ui.Pause();
            return;
        }

        UiHelpers.Pause();
        var selected = openComplaints.FirstOrDefault(c => c.Id == id);
        if (selected == null)
        {
            Console.WriteLine("Invalid ID or complaint already handled.");
            _ui.Pause();
            return;
        }

        Console.Write("\nEnter admin response message: ");
        string response = Console.ReadLine() ?? "";

        _complaintLogic.MarkComplaintHandled(id, response);

        Console.WriteLine("✔ Complaint marked as handled with admin response.");
        _ui.Pause();
    }

    private void DeleteComplaint(string location)
    {
        ViewAll(location);
        Console.Write("Enter complaint ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _complaintLogic.DeleteComplaint(id);
            Console.WriteLine("🗑 Complaint deleted.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        UiHelpers.Pause();
    }
}
