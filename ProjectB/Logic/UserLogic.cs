using System;
using System.Globalization;
using System.Linq;

public class UserLogic
{
    private readonly IUserAccess _userAccess;

    public UserLogic(IUserAccess userAccess)
    {
        _userAccess = userAccess;
    }

    public bool IsPhoneValid(string phone)
    {
        if (phone.Length == 0) return false;

        if (phone[0] != '+' && phone[0] != '0')
            return false;

        if ((phone.Trim().Substring(1, phone.Trim().Length - 1).All(char.IsDigit)
                && phone[0] == '+' && phone.Trim().Length == 12)
            || (phone.Trim().All(char.IsDigit)
                && phone[0] == '0' && phone.Length > 1 && phone[1] == '6' && phone.Trim().Length == 10))
        {
            return true;
        }

        return false;
    }
    public bool IsHeightValid(int height)
    {
        return height >= 30 && height <= 250;
    }

    public bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        int at = email.IndexOf("@");
        int dot = email.LastIndexOf(".");

        // must contain both and in correct order
        if (at <= 0) return false;                 // no text before @
        if (dot <= at + 1) return false;           // no domain name
        if (dot == email.Length - 1) return false; // no TLD

        return true;
    }


    public bool IsNameValid(string name)
    {
        if (2 < name.Length && name.Length < 20 && name.All(ch => !char.IsDigit(ch)))
            return true;

        return false;
    }

    public bool IsPasswordValid(string password)
    {
        if (password.Length < 8)
            return false;

        if (!password.Any(char.IsUpper))
            return false;

        if (!password.Any(char.IsLower))
            return false;

        if (!password.Any(char.IsDigit))
            return false;

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

        return true;
    }

    public bool IsDateOfBirthValid(string dob)
    {
        return DateTime.TryParseExact(
            dob,
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    public int DOBtoAGE(string dateOfBirth)
    {
        string[] dob = dateOfBirth.Split('-');

        int days = int.Parse(dob[0]);
        int months = int.Parse(dob[1]);
        int years = int.Parse(dob[2]);

        DateTime birthday = new DateTime(years, months, days);
        int age = (int)((DateTime.Now - birthday).TotalDays / 365);

        return age;
    }

    public void Register(string name, string email, string dateOfBirth, int height, string phone, string password)
    {
        UserModel registeredAccount = new UserModel(
            _userAccess.NextId(),
            name.ToLower(),
            email,
            dateOfBirth,
            height,
            phone,
            password);

        _userAccess.Write(registeredAccount);
    }

    public UserModel? GetById(int id) => _userAccess.GetById(id);

    public (bool ok, string? error) UpdateProfile(UserModel updated)
    {
        if (!IsNameValid(updated.Name))
            return (false, "Invalid name. It must be 3–19 characters and contain no digits.");

        if (!IsEmailValid(updated.Email))
            return (false, "Invalid email format.");

        if (!IsDateOfBirthValid(updated.DateOfBirth))
            return (false, "Invalid date of birth. Use dd-mm-yyyy and a real calendar date.");

        if (!IsHeightValid(updated.Height))
            return (false, "Invalid height. Enter a value between 30 and 250 cm.");

        if (!IsPhoneValid(updated.Phone))
            return (false, "Invalid phone. Use +########### (12 chars) or Dutch 06######## (10 chars).");

        if (!IsPasswordValid(updated.Password))
            return (false, "Invalid password. 8–16 chars with upper, lower, digit, and special.");

        try
        {
            _userAccess.Update(updated);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Could not save changes: {ex.Message}");
        }
    }

    public void DeleteUser(int id)
    {
        _userAccess.DeleteUser(id);
    }

    public string SetRole(UserModel user, bool rolechange, int adminid)
    {
        _userAccess.SetRole(user);
        return intToRole(rolechange, adminid);
    }

    private string intToRole(bool rolechange, int adminid)
    {
        string role = adminid switch
        {
            0 => "User",
            1 => "Admin",
            2 => "Superadmin",
            _ => "Unknown"
        };
        if(!rolechange)
        {
            return $"has been promoted to {role}.";
        }
        return $"has been demoted to {role}.";
    }
}
