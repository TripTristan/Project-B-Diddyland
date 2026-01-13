public class Profile
{
    private readonly Dependencies _ctx;
    private UserModel user;

    public Profile(Dependencies ctx)
    {
        _ctx = ctx;
        user = _ctx.loginStatus.CurrentUserInfo;
    }
    public void Run()
    {
        RenderProfile(user);

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Edit"},
            new List<string> {"Back"}
        };

        MainMenu Menu = new MainMenu(Options, "Options:");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        if (selectedIndex[0] == 0)
        {
            UserModel? edited = EditFlow(user);
            var (ok, error) = _ctx.userLogic.UpdateProfile(edited);

            if (edited == null || !ok)
            {
                Console.WriteLine($"{error}");
                Pause();
                return;
            }

            Console.WriteLine("Profile updated successfully.");
            Pause();
            return;
        }
    }


    private void RenderProfile(UserModel user)
    {
        Console.WriteLine($"""=== Your Profile ===\n Id          : {user.Id}\n Username    : {user.Name}\n Email       : {user.Email}\n DateOfBirth : {user.DateOfBirth}\n Height (cm) : {user.Height}\n Phone       : {user.Phone}\n Role        : {user.Admin}""");
    }

    private UserModel? EditFlow(UserModel current)
    {
        Console.Clear();
        Console.WriteLine("=== Edit Profile ===");
        Console.WriteLine("Press ENTER to keep the current value.");
        Console.WriteLine();

        UserModel draft = new UserModel
        {
            Id = current.Id,
            Name = Prompt("Username", current.Name),
            Email = Prompt("Email", current.Email),
            DateOfBirth = Prompt("Date of Birth (dd-mm-yyyy)", current.DateOfBirth),
            Height = Convert.ToInt32(Prompt("Height in cm", current.Height)),
            Phone = Prompt("Phone (+########### or 06########)", current.Phone),
            Password = Prompt("Password (leave empty to keep current)", current.Password),
            Admin = current.Admin
        };

        if(UiHelpers.ChoiceHelper("Save changes?"))
        {
            return draft;
        }

        Console.WriteLine("Canceled. No changes were saved.");
        return null;
    }

    private string Prompt<T>(string label, T current)
    {
        Console.Write($"{label} [{current.ToString()}]: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? current.ToString() : input.Trim();
    }

    private void Pause()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
