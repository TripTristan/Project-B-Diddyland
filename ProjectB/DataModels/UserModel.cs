
public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username
    {
        get => Name;
        set => Name = value;
    }
    public string Email { get; set; }
    public string DateOfBirth { get; set; }
    public int Height { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public int Admin { get; set; }

    public int Role
    {
        get => Admin;
        set => Admin = value;
    }
    public UserModel() { }

    public UserModel(int id, string name, string email, string dateOfBirth, int height, string phone, string password, int role = 0)
    {
        Id = id;
        Name = name;               
        Email = email;
        DateOfBirth = dateOfBirth;
        Height = height;
        Phone = phone;
        Password = password;
        Admin = role;             
    }
}
