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


        if (at <= 0) return false;
        if (dot <= at + 1) return false;
        if (dot == email.Length - 1) return false;

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
}
