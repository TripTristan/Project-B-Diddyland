using System;

public class ManageAdmins
{
    private readonly UserAccess _userAccess;

    public ManageAdmins(UserAccess userAccess)
    {
        _userAccess = userAccess;
    }

    public void Show()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Superadmin Control Panel ===");
            Console.WriteLine("1. View all users");
            Console.WriteLine("2. Promote user to Admin");
            Console.WriteLine("3. Demote Admin to User");
            Console.WriteLine("4. Promote Admin to Superadmin");
            Console.WriteLine("5. Demote Superadmin to Admin");
            Console.WriteLine("6. Delete user");
            Console.WriteLine("0. Back to main menu");
            Console.Write("\nChoose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllUsers();
                    break;

                case "2":
                    ChangeRole(1, "promote to Admin");
                    break;

                case "3":
                    ChangeRole(0, "demote to User");
                    break;

                case "4":
                    ChangeRole(2, "promote to Superadmin");
                    break;

                case "5":
                    ChangeRole(1, "demote to Admin");
                    break;

                case "6":
                    Delete();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    break;
            }
        }
    }

    private void ViewAllUsers()
    {
        var users = _userAccess.GetAllUsers();

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

    private void ChangeRole(int newRole, string action)
    {
        Console.Write($"Enter user ID to {action}: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _userAccess.SetRole(id, newRole);
            Console.WriteLine($"User {id} updated to role {newRole}.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }

        Pause();
    }

    private void Delete()
    {
        Console.Write("Enter user ID to delete: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _userAccess.DeleteUser(id);
            Console.WriteLine("User deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }

        Pause();
    }

    private void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}
