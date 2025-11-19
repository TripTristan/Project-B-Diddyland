public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string DateOfBirth { get; set; } = "";
    public int Height { get; set; }
    public string Phone { get; set; } = "";
    public string Password { get; set; } = "";
    public int Admin { get; set; }
    public UserLevel Level { get; set; } = UserLevel.Regular;
    public int Role { get; set; }

    public UserModel() { }

    public UserModel(int id, string name, string email, string dateOfBirth, int height, string phone, string password)
    {
        Id = id;
        Name = name;
        Username = name;
        Email = email;
        DateOfBirth = dateOfBirth;
        Height = height;
        Phone = phone;
        Password = password;
        Admin = 0;
        Level = UserLevel.Regular;
    }
}

