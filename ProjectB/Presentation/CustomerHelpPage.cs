using System;
using System.Collections.Generic;
using System.Linq;

public static class CustomerHelpPage
using System;
using System.Linq;

public class CustomerHelpPage
{
    private readonly ComplaintLogic _logic;
    private readonly LoginStatus _loginStatus;

    public CustomerHelpPage(ComplaintLogic logic, LoginStatus loginStatus)
    {
        _logic = logic;
        _loginStatus = loginStatus;
    }

    public void Show()
    {
        Console.Clear();
        Console.WriteLine("Which complaint do you have?");

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

            string username = _loginStatus.CurrentUserInfo?.Username ?? "Anonymous";
            string category = menuOptions[choice - 1];

            ComplaintLogic.SubmitComplaint(username, category, description, location, "");

            Console.WriteLine("\n✅ Your complaint has been saved. Thank you!");
            Console.WriteLine("We appreciate your feedback and will work to improve.\n");
            UiHelpers.Pause();
            return;
        }
    }

    private void ComplaintFood()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: quality@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    private void ComplaintStaff()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: hr@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    private void ComplaintSafety()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: safetycoach@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    private void ComplaintOrganization()
    {
        Console.WriteLine(@"For any small complaints, write here.
For larger complaints, you can contact us via:
  • E-mail: bob.bob@diddyland.com
  • SMS: +31 0181 982513
  • Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public void ShowHandledMessages()
    {
        string? username = _loginStatus.CurrentUserInfo?.Username;

        if (username == null || username == "Guest")
        {
            Console.WriteLine("Guests don't receive messages.");
            return;
        }

        ShowPendingMessages();

        var handledComplaints = _logic.GetByUserAndStatus(username, "Handled");

        if (!handledComplaints.Any())
            return;

        Console.WriteLine("You have some complaints that have been handled:\n");
        foreach (var c in handledComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("✅ This complaint has been handled by our staff.\n");
        }
    }

    public void ShowPendingMessages()
    {
        string username = _loginStatus.CurrentUserInfo?.Username ?? "Anonymous";
        var pendingComplaints = _logic.GetPendingByUser(username);

        if (!pendingComplaints.Any()) return;

        Console.WriteLine("You have pending complaints:\n");
        foreach (var c in pendingComplaints)
        {
            Console.WriteLine($"• {c.Description}");
            Console.WriteLine("⏳ This complaint is still pending.\n");
        }
    }
}
