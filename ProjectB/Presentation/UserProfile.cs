public class Profile
{
    private readonly UserContext _ctx;

    public Profile(UserContext ctx)
    {
        _ctx = ctx;
    }
    private UserModel user;
    public void Run()
    {
        user = _ctx.loginStatus.CurrentUserInfo;
        RenderProfile(user);

        List<List<string>> Options = new List<List<string>>
        {
            new List<string> {"Edit"},
            new List<string> {"Back"}
        };

        MainMenu Menu = new MainMenu(Options, "Options:");
        int[] selectedIndex = Menu.Run();
        UiHelpers.Pause();

        switch (selectedIndex[0])
        {
            case 0:
                var edited = EditFlow(user);
                if (edited == null)
                {
                    return;
                }
                var (ok, error) = _ctx.userLogic.UpdateProfile(edited);
                if (!ok)
                {
                    Console.WriteLine($"{error}");
                    Pause();
                    return;
                }
                Console.WriteLine("Profile updated successfully.");
                Pause();
                break;
            case 1:
                break;
            default:
                break;
        }
    }


    private void RenderProfile(UserModel user)
    {
        Console.WriteLine("=== Your Profile ===");
        Console.WriteLine($"Id          : {user.Id}");
        Console.WriteLine($"Username    : {user.Name}");
        Console.WriteLine($"Email       : {user.Email}");
        Console.WriteLine($"DateOfBirth : {user.DateOfBirth}");
        Console.WriteLine($"Height (cm) : {user.Height}");
        Console.WriteLine($"Phone       : {user.Phone}");
        Console.WriteLine($"Role        : {user.Admin}");
    }

    private UserModel? EditFlow(UserModel current)
    {
        Console.Clear();
        Console.WriteLine("=== Edit Profile ===");
        Console.WriteLine("Press ENTER to keep the current value.");
        Console.WriteLine();

        var draft = new UserModel
        {
            Id = current.Id,
            Name = Prompt("Username", current.Name),
            Email = Prompt("Email", current.Email),
            DateOfBirth = Prompt("Date of Birth (dd-mm-yyyy)", current.DateOfBirth),
            Height = PromptInt("Height in cm", current.Height),
            Phone = Prompt("Phone (+########### or 06########)", current.Phone),
            Password = PromptHiddenWithFallback("Password (leave empty to keep current)", current.Password),
            Admin = current.Admin
        };

        Console.WriteLine();
        Console.Write("Save changes? [Y]es  [N]o: ");
        var save = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
        if (save == "Y")
            return draft;

        Console.WriteLine("Canceled. No changes were saved.");
        Pause();
        if (UiHelpers.ChoiceHelper("Save changes?"))
        {
            return draft;
        }
        return null;
    }

    private string Prompt(string label, string current)
    {
        Console.Write($"{label} [{current}]: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? current : input.Trim();
    }

    private int PromptInt(string label, int current)
    {
        Console.Write($"{label} [{current}]: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
            return current;

        if (int.TryParse(input.Trim(), out var val))
            return val;

        Console.WriteLine("Invalid number. Keeping current value.");
        return current;
    }

    private string PromptHiddenWithFallback(string label, string fallback)
    {
        Console.Write($"{label}: ");
        var input = ReadPassword();
        return string.IsNullOrEmpty(input) ? fallback : input;
    }

    private string ReadPassword()
    {
        var pwd = string.Empty;
        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
            {
                pwd = pwd[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                pwd += key.KeyChar;
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return pwd;
    }

    private void Pause()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
