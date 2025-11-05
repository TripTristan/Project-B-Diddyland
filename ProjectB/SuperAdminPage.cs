static class SuperAdminPage
{
    public static void Start()
    {
        string[] menuOptions =
        {
            "Manage Admins",
            "Manage SuperAdmins",
            "View AdminPage",
            "View UserMenu",
        };

        for (int i = 0; i < menuOptions.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {menuOptions[i]}");
        }
    }
}