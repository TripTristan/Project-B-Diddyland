using System;
using System.Collections.Generic;
using System.Linq;

public static class CustomerHelpPage
{
    public static void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Which complaint do you have?");
            Console.WriteLine("0. Back to main menu");

            string[] menuOptions =
            {
                "Complaint about food",
                "Complaint about staff or service",
                "Complaint about safety",
                "Complaint about organization",
            };

            Action[] actions =
            {
                ComplaintFood,
                ComplaintStaff,
                ComplaintSafety,
                ComplaintOrganization
            };

            for (int i = 0; i < menuOptions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {menuOptions[i]}");
            }

            Console.Write("\nEnter your choice (0-4): ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > menuOptions.Length)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 4.");
                UiHelpers.Pause();
                continue;
            }

            Console.Clear();
            Console.WriteLine($"[{menuOptions[choice - 1]}]\n");
            actions[choice - 1].Invoke();

            string[] locations =
            {
                "DiddyLand - Amsterdam",
                "DiddyLand - Rotterdam"
            };

            int locChoice = 0;
            while (true)
            {
                Console.WriteLine("\nSelect park location:");
                for (int i = 0; i < locations.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {locations[i]}");
                }

                Console.Write("\nEnter location number: ");
                string locInput = Console.ReadLine();
                if (int.TryParse(locInput, out int num) && num >= 1 && num <= locations.Length)
                {
                    locChoice = num;
                    break;
                }
                Console.WriteLine("Invalid input. Please select 1 or 2.");
            }

            string location = locations[locChoice - 1];

            string description = "";
            while (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("\nPlease describe your complaint below:");
                description = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(description))
                    Console.WriteLine("Description cannot be empty.");
            }

            string username = LoginStatus.CurrentUserInfo?.Username ?? "Anonymous";
            string category = menuOptions[choice - 1];

            ComplaintLogic.SubmitComplaint(username, category, description, location, "");

            Console.WriteLine("\n✅ Your complaint has been saved. Thank you!");
            Console.WriteLine("We appreciate your feedback and will work to improve.\n");
            UiHelpers.Pause();
            return;
        }
    }

    public static void ComplaintFood()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: quality@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintStaff()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: hr@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintSafety()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: safetycoach@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintOrganization()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: bob.bob@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ShowHandledMessages()
    {
        string? username = LoginStatus.CurrentUserInfo?.Username;
        if (username == null || username == "Guest") return;

        ShowPendingMessages();
        var handledComplaints = ComplaintLogic.GetByUserAndStatus(username, "Handled");
        if (!handledComplaints.Any()) return;

        Console.WriteLine("You have some complaints that have been handled:\n");
        foreach (var c in handledComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("✅ This complaint has been handled by our staff.\n");
        }
    }

    public static void ShowPendingMessages()
    {
        string username = LoginStatus.CurrentUserInfo?.Username ?? "Anonymous";
        var pendingComplaints = ComplaintLogic.GetPendingByUser(username);
        if (!pendingComplaints.Any()) return;

        Console.WriteLine("You have pending complaints:\n");
        foreach (var c in pendingComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("⏳ This complaint is still pending.\n");
        }
    }
}
