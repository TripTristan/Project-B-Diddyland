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
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = "INSERT INTO Complaints (Id, Username, Category, Description, CreatedAt, Status , Location) " +
                         "VALUES (@Id, @Username, @Category, @Description, @CreatedAt, @Status, @Location);";
            DBC.Connection.Execute(sql, complaint);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static List<ComplaintModel> GetAll(string? location = null)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Complaints ORDER BY CreatedAt DESC;";
            if (!string.IsNullOrEmpty(location))
                sql += " WHERE Location = @Location";
            sql += " ORDER BY CreatedAt DESC;";

            List<ComplaintModel> result = DBC.Connection.Query<ComplaintModel>(sql, new { Location = location }).AsList();
            return result;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static int NextId()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = "SELECT IFNULL(MAX(Id), 0) + 1 FROM Complaints";
            int next = DBC.Connection.ExecuteScalar<int>(sql);
            return next;
        }
        catch (Exception e)
        {
            return 1;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static List<ComplaintModel> Filter(string? category = null, string? username = null, string? status = null, string? location = null)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

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

            List<ComplaintModel> result = DBC.Connection.Query<ComplaintModel>(
                sql,
                new { Category = category, Username = username, Status = status, Location = location }
            ).AsList();

            return result;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void UpdateStatus(int id, string status)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = "UPDATE Complaints SET Status = @Status WHERE Id = @Id;";
            DBC.Connection.Execute(sql, new { Id = id, Status = status });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Delete(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = "DELETE FROM Complaints WHERE Id = @Id;";
            DBC.Connection.Execute(sql, new { Id = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}
