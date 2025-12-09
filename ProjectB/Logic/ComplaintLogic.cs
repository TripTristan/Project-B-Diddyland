public class ComplaintLogic
{
    private readonly ComplaintsAccess _complaintsAccess;

    public ComplaintLogic(ComplaintsAccess complaintsAccess)
    {
        _complaintsAccess = complaintsAccess;
    }

    public void SubmitComplaint(string username, string category, string description, string location, string adminResponse)
    {
        int nextId = _complaintsAccess.NextId();

        ComplaintModel complaint = new ComplaintModel(
            nextId,
            username,
            category,
            description,
            DateTime.Now,
            "Open",
            location,
            adminResponse
        );

        _complaintsAccess.Write(complaint);
    }

    public void SubmitComplaint(string username, string category, string description, string location)
    {
        int nextId = _complaintsAccess.NextId();

        ComplaintModel complaint = new ComplaintModel(
            nextId,
            username,
            category,
            description,
            DateTime.Now,
            "Open",
            location,
            "-"
        );

        _complaintsAccess.Write(complaint);
    }
    public List<ComplaintModel> RetrieveComplaintsWithStatus(string status)
    {
        List<ComplaintModel> complaints = _complaintsAccess.Filter(status: status);
        return complaints;
    }


    public List<ComplaintModel> GetAllComplaints(string? location = null)
        => _complaintsAccess.GetAll(location);

    public List<ComplaintModel> FilterComplaints(
        string? category = null,
        string? username = null,
        string? status = null,
        string? location = null)
        => _complaintsAccess.Filter(category, username, status, location);

    public void MarkComplaintHandled(int id, string adminResponse)
    {
        if (string.IsNullOrWhiteSpace(adminResponse))
            adminResponse = "No further information provided.";

        _complaintsAccess.SetHandled(id, adminResponse);
    }

    public void DeleteComplaint(int id)
        => _complaintsAccess.Delete(id);

    public IEnumerable<ComplaintModel> GetByUserAndStatus(string username, string status)
        => _complaintsAccess.Filter(username: username, status: status);

    public IEnumerable<ComplaintModel> GetPendingByUser(string username)
        => _complaintsAccess.Filter(username: username, status: "Open");
}
