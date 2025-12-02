using Dapper;
using System;
using System.Collections.Generic;

public class AttractiesAccess : IAttractiesAccess
{
    private readonly DatabaseContext _db;

    public AttractiesAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Insert(AttractieModel attractie)
    {
        const string sql = @"INSERT INTO Attractie (Name, Type, MinHeightInCM, Capacity, Location)
                             VALUES (@Name, @Type, @MinHeightInCM, @Capacity, @Location)";

        _db.Connection.Execute(sql, attractie);
    }

    public IEnumerable<AttractieModel> GetAll(string? location = null)
    {
        string sql = "SELECT * FROM Attractie";
        if (!string.IsNullOrEmpty(location))
            sql += " WHERE Location = @Location";

        return _db.Connection.Query<AttractieModel>(sql, new { Location = location });
    }

    public AttractieModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Attractie WHERE ID = @ID";
        return _db.Connection.QueryFirstOrDefault<AttractieModel>(sql, new { ID = id });
    }

    public void Update(AttractieModel attractie)
    {
        const string sql = @"UPDATE Attractie
                             SET Name = @Name, Type = @Type, MinHeightInCM = @MinHeightInCM, Capacity = @Capacity, Location = @Location
                             WHERE ID = @ID";

        _db.Connection.Execute(sql, attractie);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Attractie WHERE ID = @ID";
        _db.Connection.Execute(sql, new { ID = id });
    }
}
