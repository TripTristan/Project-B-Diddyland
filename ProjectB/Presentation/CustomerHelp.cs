using System;

public static class CustomerHelp
{
    public static void ShowPage()
    {
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
            ComplaintOrganization,
        };

        for (int i = 0; i < menuOptions.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {menuOptions[i]}");
        }

        Console.Write("\nEnter your choice (1-4): ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= actions.Length)
        {
            Console.Clear();
            actions[choice - 1].Invoke();
        }
        else
        {
            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    public static void ComplaintFood()
    {
        Console.WriteLine(@"For any small complaints, write here.
For any larger complaints you can contact us in the following ways;

Via E-mail: quality@diddyland.com
Via SMS: +31 0181 982513
Via Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintStaff()
    {
        Console.WriteLine(@"For any small complaints, write here.
For any larger complaints you can contact us in the following ways;

Via E-mail: hr@diddyland.com
Via SMS: +31 0181 982513
Via Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintSafety()
    {
        Console.WriteLine(@"For any small complaints, write here.
For any larger complaints you can contact us in the following ways;

Via E-mail: safetycoach@diddyland.com
Via SMS: +31 0181 982513
Via Mail: 6767FN Tripisgeweldigstraat 95");
    }

    public static void ComplaintOrganization()
    {
        Console.WriteLine(@"For any small complaints, write here.
For any larger complaints you can contact us in the following ways;

Via E-mail: bob.bob@diddyland.com
Via SMS: +31 0181 982513
Via Mail: 6767FN Tripisgeweldigstraat 95");
    }
}
