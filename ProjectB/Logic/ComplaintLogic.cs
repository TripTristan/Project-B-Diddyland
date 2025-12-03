public static class ComplaintLogic
{
    public static void SubmitComplaint(string username, string category, string description, string location, string adminResponse)
    {
        int nextId = ComplaintsAccess.NextId();

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

        ComplaintsAccess.Write(complaint);
    }

    public static List<ComplaintModel> GetAllComplaints(string? location = null)
    {
        return ComplaintsAccess.GetAll(location);
    }

    public static List<ComplaintModel> FilterComplaints(string? category = null, string? username = null, string? status = null, string? location = null)
    {
        return ComplaintsAccess.Filter(category, username, status, location);
    }

    public static void MarkComplaintHandled(int id, string adminResponse)
    {
        if (string.IsNullOrWhiteSpace(adminResponse))
            adminResponse = "No further information provided.";

        ComplaintsAccess.SetHandled(id, adminResponse);
    }

    public static void DeleteComplaint(int id)
    {
        ComplaintsAccess.Delete(id);
    }

    public static IEnumerable<ComplaintModel> GetByUserAndStatus(string username, string status)
    {
        return ComplaintsAccess.Filter(username: username, status: status);
    }

    public static IEnumerable<ComplaintModel> GetPendingByUser(string username)
    {
        return ComplaintsAccess.Filter(username: username, status: "Open");
    }
}
