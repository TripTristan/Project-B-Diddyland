class UserModel{
public int Id { get; set; }
public string Name { get; set; }
public string Email { get; set; }
public string DateOfBirth { get; set; }
public int Height { get; set; }
public string Phone { get; set; }
public string Password { get; set; }
// public string Address { get; set; }

    public UserModel(int id, string name, string email, string dateOfBirth, int height, string phone, string password)
    {
        Name = name;
        Email = email;
        DateOfBirth = dateOfBirth;
        Height = height;
        Phone = phone;
        Password = password;
        Id = id;
    }


}