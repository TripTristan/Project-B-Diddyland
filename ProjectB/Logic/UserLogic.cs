using System;
using System.Collections.Generic;
using System.Linq;

public static class UserLogic
{
    public static bool IsPhoneValid(string phone)        => _phoneValid(phone);
    public static bool IsHeightValid(int height)         => _heightValid(height);
    public static bool IsEmailValid(string email)        => _emailValid(email);
    public static bool IsNameValid(string name)          => _nameValid(name);
    public static bool IsPasswordValid(string password)  => _passwordValid(password);
    public static bool IsDateOfBirthValid(string dob)    => _dobValid(dob);


    // Predicate // if valid return true else false 
    private static readonly Predicate<string> _phoneValid = phone =>
    {
        if (phone[0] != '+' && phone[0] != '0') return false;

        phone = phone.Trim();
        return (phone[0] == '+' && phone.Length == 12 && phone.Skip(1).All(char.IsDigit))
            || (phone.Length == 10 && phone[0] == '0' && phone[1] == '6' && phone.All(char.IsDigit));
    };

    private static readonly Predicate<int> _heightValid = h => h is >= 30 and <= 250;

    private static readonly Predicate<string> _emailValid = email =>
        email.Contains('@') && email.Contains('.') && email.IndexOf('@') < email.LastIndexOf('.');

    private static readonly Predicate<string> _nameValid = name =>
        name.Length is > 2 and < 20 && name.All(ch => !char.IsDigit(ch));

    private static readonly Predicate<string> _passwordValid = pwd =>
        pwd.Length is >= 8 and <= 16
        && pwd.Any(char.IsUpper)
        && pwd.Any(char.IsLower)
        && pwd.Any(char.IsDigit)
        && pwd.Any(ch => !char.IsLetterOrDigit(ch));

    private static readonly Predicate<string> _dobValid = dob =>
    {
        try
        {
            var parts = dob.Split('-').Select(int.Parse).ToArray();
            int d = parts[0], m = parts[1], y = parts[2];
            return m switch
            {
                2 => d <= 28,
                4 or 6 or 9 or 11 => d <= 30,
                _ => d <= 31
            };
        }
        catch { return false; }
    };


    public static int DOBtoAGE(string DateOfBirth)
    {
        var (d, m, y) = DateOfBirth.Split('-') switch { var a => (int.Parse(a[0]), int.Parse(a[1]), int.Parse(a[2])) };
        return (int)((DateTime.Now - new DateTime(y, m, d)).TotalDays / 365);
    }

    public static void Register(string name, string email, string dateOfBirth, int height, string phone, string password)
    {
        var user = new UserModel(UserAccess.NextId(), name, email, dateOfBirth, height, phone, password);
        UserAccess.Write(user);
    }
}