using Microsoft.Data.Sqlite;
using Dapper;

public class AttractiesAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland.db");

    public void Insert(AttractieModel attractie)
    {
        const string sql = @"INSERT INTO Attractie (Name, Type, MinHeightInCM, Capacity)
                             VALUES (@Name, @Type, @MinHeightInCM, @Capacity)";
        _connection.Execute(sql, attractie);
    }

    public IEnumerable<AttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM Attractie";
        return _connection.Query<AttractieModel>(sql);
    }

    public AttractieModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Attractie WHERE AttractieID = @AttractieID";
        return _connection.QueryFirstOrDefault<AttractieModel>(sql, new { AttractieID = id });
    }

    public void Update(AttractieModel attractie)
    {
        const string sql = @"UPDATE Attractie
                             SET Name = @Name, Type = @Type, MinHeightInCM = @MinHeightInCM, Capacity = @Capacity
                             WHERE AttractieID = @AttractieID";
        _connection.Execute(sql, attractie);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Attractie WHERE AttractieID = @AttractieID";
        _connection.Execute(sql, new { AttractieID = id });
    }
}
