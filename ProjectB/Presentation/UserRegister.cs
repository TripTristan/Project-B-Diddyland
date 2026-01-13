public class UserRegister
{
    private readonly UserLogic _userLogic;
    string name;
    string email;
    string dateOfBirth;
    string phone;
    string password;
    int height = 0;

    public UserRegister(UserLogic userLogic)
    {
        _userLogic = userLogic;
    }
    public void Register()
    {

        UsernameValidator();
        EmailValidator();
        HeightValidator();
        DateOfBirthValidator();
        PhoneValidator();
        PasswordValidator();
        Console.Clear();

        _userLogic.Register(name, email, dateOfBirth, height, phone, password);

        Console.WriteLine($"Welcome {name}!");
        Console.WriteLine("You have successfully registered your account.");
    }
    private bool UsernameValidator()
    {
        Console.Write("Please enter your username: ");
        name = Console.ReadLine();

        if (!_userLogic.IsNameValid(name))
        {
            Console.WriteLine("Invalid name! (No numbers or special characters allowed)\n");
            UsernameValidator();
        }
        return true;
    }
    private bool EmailValidator()
    {
        Console.Write("Please enter your email: ");
        email = Console.ReadLine();

        if (!_userLogic.IsEmailValid(email))
        {
            Console.WriteLine("Invalid email format! Please try again.\n");
            EmailValidator();
        }
        return true;
    }
    private bool DateOfBirthValidator()
    {
        Console.Write("Please enter your date of birth (dd-mm-yyyy): ");
        dateOfBirth = Console.ReadLine();

        if (!_userLogic.IsDateOfBirthValid(dateOfBirth))
        {
            Console.WriteLine("Invalid date! Please use dd-mm-yyyy format.\n");
            DateOfBirthValidator();
        }
        return true;
    }
    private bool HeightValidator()
    {
        Console.Write("Please enter your height in whole centimeters (e.g., 175): ");
        string heightInput = Console.ReadLine();

        if (!int.TryParse(heightInput, out height) || !_userLogic.IsHeightValid(height))
        {
            Console.WriteLine("Invalid height! Please enter a valid number in cm.\n");
            HeightValidator();
        }
        return true;
    }
    private bool PhoneValidator()
    {

        Console.Write("Please enter your phone number: ");
        phone = Console.ReadLine();
        if (!_userLogic.IsPhoneValid(phone))
        {
            Console.WriteLine("Invalid phone number format! Please try again.\n");
            PhoneValidator();
        }
        return true;

    }
    private bool PasswordValidator()
    {
        Console.Write("Please enter a valid password (8 chars min., symbol, capital letter, small letter, and a number): ");
        password = Console.ReadLine();
        if (!_userLogic.IsPasswordValid(password))
        {
            Console.WriteLine("Invalid password! Please follow the password rules.\n");
            PasswordValidator();
        }
        return true;
    }

}
