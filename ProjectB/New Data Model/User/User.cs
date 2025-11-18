
public class User
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string Nr { get; set; }
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

    public User(string nr, 
                string name, 
                string phoneNr,
                string email,
                string dateOfBirth,
                string address,
                string account,
                string password
            )
    {
        Nr = nr;
        Name = name;
        PhoneNr = phoneNr;
        Email = email;
        DateOfBirth = dateOfBirth;
        Address = address;
        Account = account;
        Password = password;
        Level = UserLevel.Regular; // Default level is Regular
        Role = ManagementRol.User; // Default role is User
    }

}
