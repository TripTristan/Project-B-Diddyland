public static class UserLogic
{
    public static bool IsPhoneValid(string phone)
    {
        if (phone[0] != '+' && phone[0] != '0')
        {
            return false;
        }

        if ((phone.Trim().Substring(1, phone.Length-1).All(ch => char.IsDigit(ch)) && phone[0] == '+' && phone.Trim().Length == 12)
        || (phone.Trim().All(ch => char.IsDigit(ch)) && phone[0] == '0' && phone[1] == '6' && phone.Trim().Length == 10))
        {
            return true;
        }

        return false;
    }

    public static bool IsHeightValid(int height)
    {
        if (height >= 30 && height <= 250)
        {
            return true;
        }
        return false;
    }

    public static bool IsEmailValid(string email)
    {
        if (!email.Contains("@") || !email.Contains("."))
        {
            return false;
        }
        if (!(email.IndexOf("@") < email.IndexOf(".")))
        {
            return false;
        }

        return true;
    }

    public static bool IsNameValid(string name)
    {
        if (2 < name.Length && name.Length < 20 && name.All(ch => !char.IsDigit(ch)))
        {
            return true;
        }
        return false;
    }

    public static bool IsPasswordValid(string Password)
    {
        if (Password.Length < 8)
            return false;

        if (!Password.Any(char.IsUpper))
            return false;

        if (!Password.Any(char.IsLower))
            return false;

        if (!Password.Any(char.IsDigit))
            return false;

        if (!Password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

    return true;
    }

    public static bool IsDateOfBirthValid(string dob)
    {
        return DateTime.TryParseExact(
            dob,
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _
        );
    }


    public static int DOBtoAGE(string DateOfBirth)
    {
        string[] dob = DateOfBirth.Split("-");

        int days = Int32.Parse(dob[0]);
        int months = Int32.Parse(dob[1]);
        int years = Int32.Parse(dob[2]);

        DateTime birthday = new DateTime(years, months, days);
        int age = (int)((DateTime.Now - birthday).TotalDays / 365);

        return age;
    }

    public static void Register(string name, string email, string dateOfBirth, int height, string phone, string password)
    {
        UserModel registeredAccount = new UserModel(UserAccess.NextId(), name, email, dateOfBirth, height, phone, password);
        UserAccess.Write(registeredAccount);
    }
}
