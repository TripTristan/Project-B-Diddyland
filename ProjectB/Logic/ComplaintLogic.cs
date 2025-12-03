public static class ComplaintLogic
{
    public static void SubmitComplaint(string username, string category, string description, string location, string adminResponse)
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

    public List<ComplaintModel> GetAllComplaints(string? location = null)
        => _complaintsAccess.GetAll(location);

    public List<ComplaintModel> FilterComplaints(
        string? category = null,
        string? username = null,
        string? status = null,
        string? location = null)
        => _complaintsAccess.Filter(category, username, status, location);

    public static void MarkComplaintHandled(int id, string adminResponse)
    {
        if (string.IsNullOrWhiteSpace(adminResponse))
            adminResponse = "No further information provided.";

        ComplaintsAccess.SetHandled(id, adminResponse);
    }

    public void DeleteComplaint(int id)
        => _complaintsAccess.Delete(id);

    public IEnumerable<ComplaintModel> GetByUserAndStatus(string username, string status)
        => _complaintsAccess.Filter(username: username, status: status);

    public IEnumerable<ComplaintModel> GetPendingByUser(string username)
        => _complaintsAccess.Filter(username: username, status: "Open");
}
