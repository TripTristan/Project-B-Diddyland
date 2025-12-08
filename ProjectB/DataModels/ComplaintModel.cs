public class ComplaintModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Open";
    public string Location { get; set; }
    public string AdminResponse { get; set; } = "";


    public ComplaintModel() { }

    public ComplaintModel(int id, string username, string category, string description, DateTime createdAt, string status, string location, string adminResponse)
    {
        Id = id;
        Username = username;
        Category = category;
        Description = description;
        CreatedAt = createdAt;
        Status = status;
        Location = location;
        AdminResponse = adminResponse;
    }
}
