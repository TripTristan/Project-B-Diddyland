public static class CustomerHelpPage
{
    public static void Show()
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

        Console.Write("\nEnter your choice (1-4): ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= menuOptions.Length)
        {
            Console.Clear();
            Console.WriteLine($"[{menuOptions[choice - 1]}]\n");

            actions[choice - 1].Invoke();

            Console.WriteLine("\nPlease describe your complaint below:");
            string description = Console.ReadLine();

            string username = LoginStatus.CurrentUserInfo?.Username ?? "Anonymous";
            string category = menuOptions[choice - 1];

            ComplaintLogic.SubmitComplaint(username, category, description);

            Console.WriteLine("\n✅ Your complaint has been saved. Thank you!");
            Console.WriteLine("We appreciate your feedback and will work to improve.\n");
        }
        else
        {
            Console.WriteLine("\nInvalid choice. Please try again.\n");
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
}