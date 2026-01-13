using System;

public class ManageAdmins
{
    private Dependencies _ctx;
    public ManageAdmins(Dependencies a) { _ctx = a; }

    public void Run()
    {

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Promote user"},
            new List<string> {"Demote user"},
            new List<string> {"Delete user"},
            new List<string> {"Back to main menu"}
        };

        MainMenu Menu = new MainMenu(Options, "=== Superadmin Control Panel ===");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        switch (selectedIndex[0])
        {
            case 0:
                ChangeRole(_ctx.SelectUser(), true);
                break;
            case 1:
                ChangeRole(_ctx.SelectUser(), false);
                break;
            case 2:
                Delete(_ctx.SelectUser());
                break;
            case 3:
                return;
            default:
                break;
        }
    }

    private void ChangeRole(UserModel user, bool roleChange)
    {
        if (roleChange && user.Admin < 2)
        {
            user.Admin++;
        }
        else if (!roleChange && user.Admin > 0)
        {
            user.Admin--;
        }
        else
        {
            Console.WriteLine("Error changing role");
            return;
        }
        string message = _ctx.userLogic.SetRole(user, roleChange, user.Admin);
        Console.WriteLine($"{user.Name} {message}");
        UiHelpers.Pause();
        return;
    }

    private void Delete(UserModel user)
    {
        UiHelpers.Error($"Are you sure you want to delete {user.Name}?");
        if(UiHelpers.ChoiceHelper(""))
        {
            _ctx.userLogic.DeleteUser(user.Id);
            Console.WriteLine("User deleted successfully.");
        }
        UiHelpers.Pause();
    }
}
