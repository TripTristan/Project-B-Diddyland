using System;
using System.Collections.Generic;
using System.Linq;

public static class AdminComplaintsPage
{
    public static void Show()
    {
        Console.WriteLine("Select park location:");
        Console.WriteLine("1) Diddyland Rotterdam");
        Console.WriteLine("2) Diddyland Amsterdam");
        string choice = Console.ReadLine() ?? "1";
        string location = choice == "2" ? "DiddyLand - Amsterdam" : "DiddyLand - Rotterdam";

        while (true)
        {
            Console.Clear();
            UiHelpers.WriteHeader($"Admin – Complaint Management ({location})");
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
                    UiHelpers.Warn("Invalid choice.");
                    UiHelpers.Pause();
                    break;
            }
        }
    }

    private static void ViewAll(string location)
    {
        var complaints = ComplaintsAccess.Filter(location: location);
        Console.Clear();
        UiHelpers.WriteHeader($"All Complaints ({location})");
        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void FilterByCategory(string location)
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
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }

        Console.Write("\nEnter category number (1–4): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 1 || choice > categories.Length)
        {
            UiHelpers.Warn("Invalid choice. Please enter a number between 1 and 4.");
            UiHelpers.Pause();
            return;
        }

        string category = categories[choice - 1];
        var complaints = ComplaintsAccess.Filter(category: category, location: location);

        Console.Clear();
        UiHelpers.WriteHeader($"Complaints in category: {category} ({location})");
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
        UiHelpers.Pause();
    }

    private static void FilterByUser(string location)
    {
        var usernames = ComplaintsAccess.Filter(location: location)
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
        var complaints = ComplaintsAccess.Filter(username: selectedUser, location: location);

        Console.Clear();
        UiHelpers.WriteHeader($"Complaints by {selectedUser} ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void FilterByStatus(string location)
    {
        var statuses = ComplaintsAccess.Filter(location: location)
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

        Console.Write("\nEnter number: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > statuses.Count)
        {
            UiHelpers.Warn("Invalid choice.");
            UiHelpers.Pause();
            return;
        }

        string selectedStatus = statuses[choice - 1];
        var complaints = ComplaintsAccess.Filter(status: selectedStatus, location: location);

        Console.Clear();
        UiHelpers.WriteHeader($"Complaints with status {selectedStatus} ({location})");

        foreach (var c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void MarkHandled(string location)
    {
        if (LoginStatus.CurrentUserInfo?.Username == "Guest")
        {
            Console.WriteLine("Guests cannot handle complaints.");
            UiHelpers.Pause();
            return;
        }

        List<ComplaintModel> openComplaints =
            ComplaintsAccess.Filter(location: location, status: "Open");

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




    private static void DeleteComplaint(string location)
    {
        ViewAll(location);
        Console.Write("Enter complaint ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            ComplaintsAccess.Delete(id);
            Console.WriteLine("🗑 Complaint deleted.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        UiHelpers.Pause();
    }
}
