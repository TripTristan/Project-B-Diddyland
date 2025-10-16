using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

public class SessionAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland2.db");

    public List<Session> GetAllSessions()
    {
        return _connection.Query<Session>("SELECT * FROM Sessions").ToList();
    }

    public Session? GetSessionById(int id)
    {
        return _connection.QueryFirstOrDefault<Session>(
            "SELECT * FROM Sessions WHERE Id = @Id", new { Id = id });
    }

    public void UpdateSession(Session session)
    {
        _connection.Execute(
            @"UPDATE Sessions
              SET CurrentBookings = @CurrentBookings
              WHERE Id = @Id",
            session);
    }
}