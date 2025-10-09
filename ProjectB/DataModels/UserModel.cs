public class UserModel
{
    public int AccountID { get; set; }
    public int Admin { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int HeightInCM { get; set; }
    public string DateOfBirth { get; set; } = "";
}