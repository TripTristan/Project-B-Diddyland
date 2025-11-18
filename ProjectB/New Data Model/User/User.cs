
public class User
{
    public int Id { get; set; } // Primary Key// Database Generated

    public string UserNr { get; set; }
    public string Name { get; set; }
    public string PhoneNr { get; set; }
    public string Email { get; set; }
    public string DateOfBirth { get; set; }
    public string Address { get; set; }


    public string Account { get; set; }
    public string Password { get; set; }
    public UserLevel Level { get; set; } = UserLevel.Regular;
    public ManagementRol Role { get; set; } = ManagementRol.User;
    // public string Role { get; set; } = "User"; 
    //  Default role is User, can be "Admin"
    // public string Address { get; set; }

    public User(string userNr, 
                string name, 
                string phoneNr,
                string email,
                string dateOfBirth,
                string address,
                string account,
                string password
            )
    {
        UserNr = userNr;
        Name = name;
        PhoneNr = phoneNumber;
        Email = email;
        DateOfBirth = dateOfBirth;
        Address = address;
        Account = account;
        Password = password;
        Level = level; // Default level is Regular
        Role = role; // Default role is User
    }

}
