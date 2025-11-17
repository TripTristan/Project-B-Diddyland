static class AdminComplaintsPage
{
    public static void Show()
    {
        while (true)
        {
            Console.Clear();
            UiHelpers.WriteHeader("Admin – Complaint Management");
            Console.WriteLine("1) View all complaints");
            Console.WriteLine("2) Filter by category");
            Console.WriteLine("3) Filter by username");
            Console.WriteLine("4) Filter by status");
            Console.WriteLine("5) Mark complaint as handled");
            Console.WriteLine("6) Delete complaint");
            Console.WriteLine("0) Back");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAll();
                    break;
                case "2":
                    FilterByCategory();
                    break;
                case "3":
                    FilterByUser();
                    break;
                case "4":
                    FilterByStatus();
                    break;
                case "5":
                    MarkHandled();
                    break;
                case "6":
                    DeleteComplaint();
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

    private static void ViewAll()
    {
        List<ComplaintModel> complaints = ComplaintsAccess.GetAll();
        Console.Clear();
        UiHelpers.WriteHeader("All Complaints");
        foreach (ComplaintModel c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void FilterByCategory()
    {
    Console.Clear();
    UiHelpers.WriteHeader("Filter Complaints by Category");

    Console.WriteLine("Available categories:");
    Console.WriteLine("1) Complaint about food");
    Console.WriteLine("2) Complaint about staff or service");
    Console.WriteLine("3) Complaint about safety");
    Console.WriteLine("4) Complaint about organization");
    Console.WriteLine();

    Console.Write("Enter category number (1–4): ");
    string? input = Console.ReadLine();
    string? category = null;

    switch (input)
    {
        case "1":
            category = "Complaint about food";
            break;
        case "2":
            category = "Complaint about staff or service";
            break;
        case "3":
            category = "Complaint about safety";
            break;
        case "4":
            category = "Complaint about organization";
            break;
        default:
            UiHelpers.Warn("Invalid choice. Please enter a number between 1 and 4.");
            UiHelpers.Pause();
            return;
    }

    List<ComplaintModel> complaints = ComplaintsAccess.Filter(category: category);

    Console.Clear();
    UiHelpers.WriteHeader($"Complaints in category: {category}");

    if (complaints.Count == 0)
    {
        Console.WriteLine("No complaints found for this category.");
    }
    else
    {
        foreach (ComplaintModel c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
    }

    UiHelpers.Pause();
    }


    private static void FilterByUser()
    {
    List<string> usernames = UserAccess.GetAllUsernames()
                                       .Distinct()
                                       .OrderBy(u => u)
                                       .ToList();

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

    List<ComplaintModel> complaints = ComplaintsAccess.Filter(username: selectedUser);

    Console.Clear();
    UiHelpers.WriteHeader($"Complaints by {selectedUser}");

    foreach (ComplaintModel c in complaints)
    {
        Console.WriteLine($"[{c.Id}] {c.Category} - {c.Status}");
        Console.WriteLine($"    {c.Description}");
        Console.WriteLine();
    }

    UiHelpers.Pause();
    }


    private static void FilterByStatus()
    {
        Console.Write("Enter status (e.g. Open or Handled): ");
        string? status = Console.ReadLine();
        List<ComplaintModel> complaints = ComplaintsAccess.Filter(status: status);
        Console.Clear();
        UiHelpers.WriteHeader($"Complaints with status {status}");
        foreach (ComplaintModel c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Category}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void MarkHandled()
    {
        Console.Write("Enter complaint ID to mark as handled: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            ComplaintsAccess.UpdateStatus(id, "Handled");
            Console.WriteLine("✅ Complaint marked as handled.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        UiHelpers.Pause();
    }

    private static void DeleteComplaint()
    {
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
