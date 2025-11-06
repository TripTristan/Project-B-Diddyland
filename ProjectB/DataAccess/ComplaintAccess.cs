using Microsoft.Data.Sqlite;
using Dapper;
using System;
using System.Collections.Generic;

public static class ComplaintsAccess
{
    public static void Write(ComplaintModel complaint)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "INSERT INTO Complaints (Id, Username, Category, Description, CreatedAt, Status) " +
                         "VALUES (@Id, @Username, @Category, @Description, @CreatedAt, @Status);";
            DBC.Connection.Execute(sql, complaint);
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static List<ComplaintModel> GetAll()
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Complaints ORDER BY CreatedAt DESC;";
            List<ComplaintModel> result = DBC.Connection.Query<ComplaintModel>(sql).AsList();
            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static int NextId()
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT IFNULL(MAX(Id), 0) + 1 FROM Complaints";
            int next = DBC.Connection.ExecuteScalar<int>(sql);
            return next;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static List<ComplaintModel> Filter(string? category = null, string? username = null, string? status = null)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Complaints WHERE 1=1";

            if (!string.IsNullOrEmpty(category))
                sql += " AND Category = @Category";

            if (!string.IsNullOrEmpty(username))
                sql += " AND Username = @Username";

            if (!string.IsNullOrEmpty(status))
                sql += " AND Status = @Status";

            sql += " ORDER BY CreatedAt DESC;";

            List<ComplaintModel> result = DBC.Connection.Query<ComplaintModel>(
                sql,
                new { Category = category, Username = username, Status = status }
            ).AsList();

            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static void UpdateStatus(int id, string status)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "UPDATE Complaints SET Status = @Status WHERE Id = @Id;";
            DBC.Connection.Execute(sql, new { Id = id, Status = status });
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static void Delete(int id)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "DELETE FROM Complaints WHERE Id = @Id;";
            DBC.Connection.Execute(sql, new { Id = id });
        }
        finally
        {
            DBC.CloseConnection();
        }
    }
}