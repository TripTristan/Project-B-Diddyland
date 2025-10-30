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
        if (Password.Length < 8 ||
            Password.Length > 16 ||
            !Password.Any(ch => !char.IsLower(ch)) ||
            !Password.Any(ch => !char.IsDigit(ch)) ||
            !Password.Any(ch => !char.IsLetterOrDigit(ch)) ||
            !Password.Any(ch => !char.IsUpper(ch))
           )
            { return false; }
        return true;
    }

    public static bool IsDateOfBirthValid(string DOB)
    {
        string[] dob = DOB.Split("-");

        try
        {
            int days = Int32.Parse(dob[0]);
            int months = Int32.Parse(dob[1]);
            int years = Int32.Parse(dob[2]);

            List<int> longMonths = new() { 1, 3, 5, 7, 8, 10, 12 };
            if (longMonths.Contains(months) && days <= 31)
            {
                return true;
            }
            if (months == 2 && days <= 28)
            {
                return true;
            }
            if (months < 12 && months > 1 && days <= 30)
            {
                return true;
            }
            return false;
        }

        catch (Exception e)
        {
            return false;
        }
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
        UserAccess UA = new();
        UserModel registeredAccount = new UserModel(UA.NextId(), name, email, dateOfBirth, height, phone, password);
        UA.Write(registeredAccount);
    }
}
