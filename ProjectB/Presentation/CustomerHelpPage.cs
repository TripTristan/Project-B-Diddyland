public static class CustomerHelpPage
{
    public static void Show()
    {
        Console.Clear();

        List<List<string>> Options = new List<List<string>> 
        {
            new List<string> {"Complaint about food"},
            new List<string> {"Complaint about staff or service"}, 
            new List<string> {"Complaint about safety"}, 
            new List<string> {"Complaint about organization"}
        };
        MainMenu Menu = new MainMenu(Options, "Which complaint do you have?");
        int[] selectedIndex = Menu.Run();
        Console.WriteLine($"{selectedIndex[0]} {selectedIndex[1]}");
        UiHelpers.Pause();

        Console.Clear();


        switch (selectedIndex[0])
        {
            case 0:
                Complain("quality@diddyland.com");
                break;
            case 1:
                Complain(" hr@diddyland.com");
                break;
            case 2:
                Complain("safetycoach@diddyland.com");
                break;
            case 3:
                Complain("bob.bob@diddyland.com");
                break;
            default:
                break;
        }

  
        Console.WriteLine("\nPlease describe your complaint below:");
        string description = Console.ReadLine();

        string username = LoginStatus.CurrentUserInfo?.Username ?? "Anonymous";
        string category = Options[selectedIndex[0]][0];

        ComplaintLogic.SubmitComplaint(username, category, description);

        Console.WriteLine("\n✅ Your complaint has been saved. Thank you!");
        Console.WriteLine("We appreciate your feedback and will work to improve.\n");

    }

    public static void Complain(string mail)
    {
        Console.WriteLine($@"For any small complaints, write here.
    For larger complaints, you can contact us via:
    • E-mail: {mail}
    • SMS: +31 0181 982513
    • Mail: 6767FN Tripisgeweldigstraat 95");
    }
}
