using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using Dapper;
using System.Data;

public class AttractiesAccess
{
    DBC db = new();
public void Insert(AttractieModel attractie)
{
    try
    {
        using (var connection = db.Connection)
        {
            connection.Open();
            const string sql = @"INSERT INTO Attractie (Name, Type, MinHeightInCM, Capacity)
                             VALUES (@Name, @Type, @MinHeightInCM, @Capacity)";
            db.Connection.Execute(sql, attractie);
        }
    }
    catch (Exception ex)
    {
        // Log any database-related errors
        Console.WriteLine($"Error during insert: {ex.Message}");
        throw;
    }
}

    public IEnumerable<AttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM Attractie";
        return db.Connection.Query<AttractieModel>(sql);
    }

    public AttractieModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Attractie WHERE ID = @ID";
        return db.Connection.QueryFirstOrDefault<AttractieModel>(sql, new { ID = id });
    }

    public void Update(AttractieModel attractie)
    {
        const string sql = @"UPDATE Attractie
                             SET Name = @Name, Type = @Type, MinHeightInCM = @MinHeightInCM, Capacity = @Capacity
                             WHERE ID = @ID";
        db.Connection.Execute(sql, attractie);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Attractie WHERE ID = @ID";
        db.Connection.Execute(sql, new { ID = id });
    }
}