using System;
using System.Collections.Generic;

public static class ComplaintLogic
{
    public static void SubmitComplaint(string username, string category, string description)
    {
        int nextId = ComplaintsAccess.NextId();

        ComplaintModel complaint = new ComplaintModel(
            nextId,
            username,
            category,
            description,
            DateTime.Now,
            "Open"
        );

        ComplaintsAccess.Write(complaint);
    }

    public static List<ComplaintModel> GetAllComplaints()
    {
        return ComplaintsAccess.GetAll();
    }

    public static List<ComplaintModel> FilterComplaints(string? category = null, string? username = null, string? status = null)
    {
        return ComplaintsAccess.Filter(category, username, status);
    }

    public static void UpdateStatus(int id, string status)
    {
        ComplaintsAccess.UpdateStatus(id, status);
    }

    public static void DeleteComplaint(int id)
    {
        ComplaintsAccess.Delete(id);
    }
}