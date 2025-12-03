using System;
using System.Collections.Generic;
using System.Linq;

public class AdminComplaintsPage
{
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
            _ui.WriteHeader($"Admin – Complaint Management ({location})");
            Console.WriteLine("1) View all complaints");
            Console.WriteLine("2) Filter by category");
            Console.WriteLine("3) Filter by username");
            Console.WriteLine("4) Filter by status");
            Console.WriteLine("5) Mark complaint as handled");
            Console.WriteLine("6) Delete complaint");
            Console.WriteLine("0) Back");

            Console.Write("\nChoose an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ViewAll(location);
                    break;
                case "2":
                    FilterByCategory(location);
                    break;
                case "3":
                    FilterByUser(location);
                    break;
                case "4":
                    FilterByStatus(location);
                    break;
                case "5":
                    MarkHandled(location);
                    break;
                case "6":
                    DeleteComplaint(location);
                    break;
                case "0":
                    return;
                default:
                    _ui.Warn("Invalid choice.");
                    _ui.Pause();
                    break;
            }
        }
    }

    private void ViewAll(string location)
    {
        var complaints = _complaintLogic.FilterComplaints(location: location);
        Console.Clear();
        _ui.WriteHeader($"All Complaints ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        _ui.Pause();
    }

    private void FilterByCategory(string location)
    {
        Console.Clear();
        _ui.WriteHeader("Filter Complaints by Category");

        string[] categories =
        {
            "Complaint about food",
            "Complaint about staff or service",
            "Complaint about safety",
            "Complaint about organization"
        };

        Console.WriteLine("Available categories:");
        for (int i = 0; i < categories.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }

        Console.Write("\nEnter category number (1–4): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 1 || choice > categories.Length)
        {
            _ui.Warn("Invalid choice. Please enter a number between 1 and 4.");
            _ui.Pause();
            return;
        }

        string category = categories[choice - 1];
        var complaints = _complaintLogic.FilterComplaints(category: category, location: location);

        Console.Clear();
        _ui.WriteHeader($"Complaints in category: {category} ({location})");
        if (complaints.Count == 0)
        {
            Console.WriteLine("No complaints found for this category.");
        }
        else
        {
            foreach (var c in complaints)
            {
                Console.WriteLine($"[{c.Id}] {c.Username} - {c.Status}");
                Console.WriteLine($"    {c.Description}");
                Console.WriteLine();
            }
        }
        _ui.Pause();
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
            _ui.Pause();
            return;
        }

        Console.Clear();
        _ui.WriteHeader("Select a Username");

        for (int i = 0; i < usernames.Count; i++)
            Console.WriteLine($"{i + 1}. {usernames[i]}");

        Console.Write("\nEnter number: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > usernames.Count)
        {
            _ui.Warn("Invalid choice.");
            _ui.Pause();
            return;
        }

        string selectedUser = usernames[choice - 1];
        var complaints = _complaintLogic.FilterComplaints(username: selectedUser, location: location);

        Console.Clear();
        _ui.WriteHeader($"Complaints by {selectedUser} ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        _ui.Pause();
    }

    private void FilterByStatus(string location)
    {
        var statuses = _complaintLogic
            .FilterComplaints(location: location)
            .Select(c => c.Status)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        if (statuses.Count == 0)
        {
            Console.WriteLine("No statuses found for this location.");
            _ui.Pause();
            return;
        }

        Console.Clear();
        _ui.WriteHeader("Select a Status");

        for (int i = 0; i < statuses.Count; i++)
            Console.WriteLine($"{i + 1}. {statuses[i]}");

        Console.Write("\nEnter number: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > statuses.Count)
        {
            _ui.Warn("Invalid choice.");
            _ui.Pause();
            return;
        }

        string selectedStatus = statuses[choice - 1];
        var complaints = _complaintLogic.FilterComplaints(status: selectedStatus, location: location);

        Console.Clear();
        _ui.WriteHeader($"Complaints with status {selectedStatus} ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        _ui.Pause();
    }

    private void MarkHandled(string location)
    {
        List<ComplaintModel> openComplaints = _complaintLogic.FilterComplaints(location: location, status: "Open");

        if (openComplaints.Count == 0)
        {
            Console.WriteLine("No open complaints to handle.");
            _ui.Pause();
            return;
        }

        Console.Clear();
        _ui.WriteHeader("Open Complaints");
        foreach (var c in openComplaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }

        Console.Write("Enter complaint ID to mark as handled: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            UiHelpers.Pause();
            return;
        }

        var selected = openComplaints.FirstOrDefault(c => c.Id == id);
        if (selected == null)
        {
            Console.WriteLine("Invalid ID or complaint already handled.");
            UiHelpers.Pause();
            return;
        }

        Console.Write("\nEnter admin response message: ");
        string response = Console.ReadLine() ?? "";

        ComplaintLogic.MarkComplaintHandled(id, response);

        Console.WriteLine("✔ Complaint marked as handled with admin response.");
        UiHelpers.Pause();
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
        _ui.Pause();
    }
}
