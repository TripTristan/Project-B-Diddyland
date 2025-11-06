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
        Console.Write("Enter category: ");
        string? category = Console.ReadLine();
        List<ComplaintModel> complaints = ComplaintsAccess.Filter(category: category);
        Console.Clear();
        UiHelpers.WriteHeader($"Complaints in category: {category}");
        foreach (ComplaintModel c in complaints)
        {
            Console.WriteLine($"[{c.Id}] {c.Username} - {c.Status}");
            Console.WriteLine($"    {c.Description}");
            Console.WriteLine();
        }
        UiHelpers.Pause();
    }

    private static void FilterByUser()
    {
        Console.Write("Enter username: ");
        string? username = Console.ReadLine();
        List<ComplaintModel> complaints = ComplaintsAccess.Filter(username: username);
        Console.Clear();
        UiHelpers.WriteHeader($"Complaints by {username}");
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
