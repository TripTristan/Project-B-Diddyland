static class AdminComplaintsPage
{
    static string status;
    public static void Show()
    {
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
                    ViewAll();
                    break;
                case 1:
                    FilterByCategory();
                    break;
                case 2:
                    FilterByUser();
                    break;
                case 3:
                    FilterByStatus();
                    break;
                case 4:
                    MarkHandled();
                    break;
                case 5:
                    DeleteComplaint();
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
