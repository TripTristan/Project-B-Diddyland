static class AdminPage
{
    public static void Start()
    {
        string[] menuOptions =
        {
            "Manage users",
            "Manage Menu's",
            "Manage Customer Service",
            "Manage Opening Hours",
            "View UserMenu",
        };

        for (int i = 0; i < menuOptions.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {menuOptions[i]}");
        }
    }
}