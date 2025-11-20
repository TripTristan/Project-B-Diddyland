using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using Dapper;
using System.Data;

public static class AttractiesAccess
{
    public static void Insert(AttractieModel attractie)
    {
        try
        {
            using (var connection = DBC.Connection)
            {
                connection.Open();
                const string sql = @"INSERT INTO Attractie (Name, Type, MinHeightInCM, Capacity, Location)
                                     VALUES (@Name, @Type, @MinHeightInCM, @Capacity, @Location)";
                connection.Execute(sql, attractie);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during insert: {ex.Message}");
            throw;
        }
    }

    public static IEnumerable<AttractieModel> GetAll(string? location = null)
    {
        string sql = "SELECT * FROM Attractie";
        if (!string.IsNullOrEmpty(location))
            sql += " WHERE Location = @Location";
        return DBC.Connection.Query<AttractieModel>(sql, new { Location = location });
    }

    public static AttractieModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Attractie WHERE ID = @ID";
        return DBC.Connection.QueryFirstOrDefault<AttractieModel>(sql, new { ID = id });
    }

    public static void Update(AttractieModel attractie)
    {
        const string sql = @"UPDATE Attractie
                             SET Name = @Name, Type = @Type, MinHeightInCM = @MinHeightInCM, Capacity = @Capacity, Location = @Location
                             WHERE ID = @ID";
        DBC.Connection.Execute(sql, attractie);
    }

    public static void Delete(int id)
    {
        const string sql = "DELETE FROM Attractie WHERE ID = @ID";
        DBC.Connection.Execute(sql, new { ID = id });
    }
}