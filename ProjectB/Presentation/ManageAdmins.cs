using System;

public static class ManageAdmins
{
    public static void Show()
    {
        bool running = true;

        while (running)
        {
            string Prompt = "=== Superadmin Control Panel ===";

            List<List<string>> Options = new List<List<string>> 
            {
                new List<string> {"View all users"},
                new List<string> {"Promote user to Admin"},       
                new List<string> {"Demote Admin to User"},
                new List<string> {"Promote Admin to Superadmin"},
                new List<string> {"Demote Superadmin to Admin"},
                new List<string> {"Delete user"},
                new List<string> {"Back to main menu"}
            };

            MainMenu Menu = new MainMenu(Options, Prompt);
            int[] selectedIndex = Menu.Run();
            UiHelpers.Pause();

            switch (selectedIndex[0])
            {
                case 0:
                    ViewAllUsers();
                    break;
                case 1:
                    ChangeRole(1, "promote to Admin");
                    break;
                case 2:
                    ChangeRole(0, "demote to User");
                    break;
                case 3:
                    ChangeRole(2, "promote to Superadmin");
                    break;
                case 4:
                    ChangeRole(1, "demote to Admin");
                    break;
                case 5:
                    Delete();
                    break;
                case 6:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    break;
            }
        }
    }

    private static void ViewAllUsers()
    {
        var users = UserAccess.GetAllUsers();
        Console.Clear();
        Console.WriteLine("=== All Users ===");
        foreach (var user in users)
        {
            string role = user.Admin switch
            {
                0 => "User",
                1 => "Admin",
                2 => "Superadmin",
                _ => "Unknown"
            };
            Console.WriteLine($"{user.Id,-4} {user.Username,-20} {role}");
        }
        Pause();
    }

    private static void ChangeRole(int newRole, string action)
    {
        Console.Write($"Enter user ID to {action}: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            UserAccess.SetRole(id, newRole);
            Console.WriteLine($"User {id} updated to role {newRole}.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Pause();
    }

    private static void Delete()
    {
        Console.Write("Enter user ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            UserAccess.DeleteUser(id);
            Console.WriteLine("User deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}