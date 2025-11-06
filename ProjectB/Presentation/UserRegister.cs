static class UserRegister
{
    public static string Register()
    {
        string Name = "";
        string Email = "";
        string DateOfBirth = "";
        string Phone = "";
        string Password = "";
        int Height = 0;

        do
        {
            Console.Write("Please enter your username: ");
            Name = Console.ReadLine();
            if (!UserLogic.IsNameValid(Name))
            {
                Console.WriteLine("Invalid name! (No numbers or special characters allowed)\n");
            }
        } while (!UserLogic.IsNameValid(Name));

        do
        {
            Console.Write("Please enter your email: ");
            Email = Console.ReadLine();
            if (!UserLogic.IsEmailValid(Email))
            {
                Console.WriteLine("Invalid email format! Please try again.\n");
            }
        } while (!UserLogic.IsEmailValid(Email));

        do
        {
            Console.Write("Please enter your date of birth (dd-mm-yyyy): ");
            DateOfBirth = Console.ReadLine();
            if (!UserLogic.IsDateOfBirthValid(DateOfBirth))
            {
                Console.WriteLine("Invalid date! Please use dd-mm-yyyy format.\n");
            }
        } while (!UserLogic.IsDateOfBirthValid(DateOfBirth));

        do
        {
            Console.Write("Please enter your height in whole centimeters (e.g., 175): ");
            string heightInput = Console.ReadLine();

            if (!int.TryParse(heightInput, out Height) || !UserLogic.IsHeightValid(Height))
            {
                Console.WriteLine("Invalid height! Please enter a valid number in cm.\n");
            }
        } while (!UserLogic.IsHeightValid(Height));

        do
        {
            Console.Write("Please enter your phone number: ");
            Phone = Console.ReadLine();
            if (!UserLogic.IsPhoneValid(Phone))
            {
                Console.WriteLine("Invalid phone number format! Please try again.\n");
            }
        } while (!UserLogic.IsPhoneValid(Phone));

        do
        {
            Console.Write("Please enter a valid password (8–16 chars, symbol, capital letter, small letter, and a number): ");
            Password = Console.ReadLine();
            if (!UserLogic.IsPasswordValid(Password))
            {
                Console.WriteLine("Invalid password! Please follow the password rules.\n");
            }
        } while (!UserLogic.IsPasswordValid(Password));

        Console.Clear();
        UserLogic.Register(Name, Email, DateOfBirth, Height, Phone, Password);

        return $"Welcome {Name}!\nYou have successfully registered your account.";
    }
}