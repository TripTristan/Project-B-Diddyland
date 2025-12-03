using System;
using System.Collections.Generic;
using Dapper;

public class ComplaintsAccess
{
    private readonly DatabaseContext _db;

    public ComplaintsAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Write(ComplaintModel complaint)
    {
        const string sql = @"
            INSERT INTO Complaints (Id, Username, Category, Description, CreatedAt, Status, Location)
            VALUES (@Id, @Username, @Category, @Description, @CreatedAt, @Status, @Location);";

        _db.Connection.Execute(sql, complaint);
    }

    public List<ComplaintModel> GetAll(string? location = null)
    {
        string sql = "SELECT * FROM Complaints";
        if (!string.IsNullOrEmpty(location))
        {
            sql += " WHERE Location = @Location";
        }
        sql += " ORDER BY CreatedAt DESC;";

        var result = _db.Connection.Query<ComplaintModel>(sql, new { Location = location }).AsList();
        return result;
    }

    public int NextId()
    {
        try
        {
            const string sql = "SELECT IFNULL(MAX(Id), 0) + 1 FROM Complaints";
            int next = _db.Connection.ExecuteScalar<int>(sql);
            return next;
        }
        catch
        {
            return 1;
        }
    }

    public List<ComplaintModel> Filter(
        string? category = null,
        string? username = null,
        string? status = null,
        string? location = null)
    {
        string sql = "SELECT * FROM Complaints WHERE 1=1";

        if (!string.IsNullOrEmpty(category))
            sql += " AND Category = @Category";

        if (!string.IsNullOrEmpty(username))
            sql += " AND Username = @Username";

        if (!string.IsNullOrEmpty(status))
            sql += " AND Status = @Status";

        if (!string.IsNullOrEmpty(location))
            sql += " AND Location = @Location";

        sql += " ORDER BY CreatedAt DESC;";

        var result = _db.Connection.Query<ComplaintModel>(
            sql,
            new { Category = category, Username = username, Status = status, Location = location }
        ).AsList();

        return result;
    }

    public static void SetHandled(int id, string adminResponse)
    {
        const string sql = @"
            UPDATE Complaints
            SET Status = 'Handled',
                AdminResponse = @AdminResponse
            WHERE ID = @Id";

        DBC.Connection.Execute(sql, new { Id = id, AdminResponse = adminResponse });
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Complaints WHERE Id = @Id;";
        _db.Connection.Execute(sql, new { Id = id });
    }
}
