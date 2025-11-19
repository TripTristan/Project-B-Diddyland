using Microsoft.Data.Sqlite;
using Dapper;
using System.Data;

public static class AttractiesAccess
{
    public static void Insert(AttractieModel attractie)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO Attractie (Name, Type, MinHeightInCM, Capacity)
                                VALUES (@Name, @Type, @MinHeightInCM, @Capacity)";
            DBC.Connection.Execute(sql, attractie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during insert: {ex.Message}");
            throw;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<AttractieModel> GetAll()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Attractie";
            return DBC.Connection.Query<AttractieModel>(sql);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static AttractieModel? GetById(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Attractie WHERE ID = @ID";
            return DBC.Connection.QueryFirstOrDefault<AttractieModel>(sql, new { ID = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Update(AttractieModel attractie)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"UPDATE Attractie
                                 SET Name = @Name, Type = @Type, MinHeightInCM = @MinHeightInCM, Capacity = @Capacity
                                 WHERE ID = @ID";
            DBC.Connection.Execute(sql, attractie);
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

            const string sql = "DELETE FROM Attractie WHERE ID = @ID";
            DBC.Connection.Execute(sql, new { ID = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

}