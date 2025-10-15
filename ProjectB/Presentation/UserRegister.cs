static class UserRegister
{
    public static string Register()
    {
        bool VDateOfBirth = false;
        bool VPassword = false;
        bool VHeight = false;
        bool VEmail = false;
        bool VPhone = false;
        bool VName = false;

        string DateOfBirth = "";
        string Password = "";
        string Email = "";
        string Phone = "";
        string Name = "";
        int Height = 0;

        while (!VName || !VEmail || !VDateOfBirth || !VHeight || !VPhone || !VPassword)
        {
            if (!VName)
            {
                Console.WriteLine("Please enter your username: ");
                Name = Console.ReadLine();
                VName = UserLogic.IsNameValid(Name);
            }

            if (!VEmail)
            {
                Console.WriteLine("Please enter your email: ");
                Email = Console.ReadLine();
                VEmail = UserLogic.IsEmailValid(Email);
            }

            if (!VDateOfBirth)
            {
                Console.WriteLine("Please enter a your date of birth (dd-mm-yyyy): ");
                DateOfBirth = Console.ReadLine();
                VDateOfBirth = UserLogic.IsDateOfBirthValid(DateOfBirth);
            }

            if (!VHeight)
            {
                Console.WriteLine("Please enter your height in whole centimeters (175 cm): ");
                try
                {
                    Height = Int32.Parse(Console.ReadLine());
                    VHeight = UserLogic.IsHeightValid(Height);
                }
                catch (Exception e)
                {
                    VHeight = false;
                }
            }

            if (!VPhone)
            {
                Console.WriteLine("Please enter your phone number: ");
                Phone = Console.ReadLine();
                VPhone = UserLogic.IsPhoneValid(Phone);
            }

            if (!VPassword)
            {
                Console.WriteLine("Please enter a valid password (8-16 chars, symbol, capital letter, small letter and a number): ");
                Password = Console.ReadLine();
                VPassword = UserLogic.IsPasswordValid(Password);
            }

            Console.Clear();
            Console.WriteLine("The following info is invalid");

        }

        Console.Clear();
        UserLogic.Register(Name, Email, DateOfBirth, Height, Phone, Password);
        return $"Welcome {Name}!\nYou have successfully registered your account.";

        
    }
}


/*
Presentation Layer

 Display registration form.
 Required fields should be name, age, email, height, and optionally phone & address.
 Show success or error messages on submit.
 */