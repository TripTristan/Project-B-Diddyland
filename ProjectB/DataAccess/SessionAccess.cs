using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

public class SessionAccess
{
    DBC db = new();

    public List<Session> GetAllSessions()
    {
        return db.Connection.Query<Session>("SELECT * FROM Sessions").ToList();
    }

    public Session? GetSessionById(int id)
    {
        return db.Connection.QueryFirstOrDefault<Session>(
            "SELECT * FROM Sessions WHERE Id = @Id", new { Id = id });
    }

    public void UpdateSession(Session session)
    {
        db.Connection.Execute(
            @"UPDATE Sessions
              SET CurrentBookings = @CurrentBookings
              WHERE Id = @Id",
            session);
    }
        public void Insert(Session session)
    {
        const string sql = @"INSERT INTO Sessions (Date, Time, MaxCapacity, CurrentBookings)
                             VALUES (@Date, @Time, @MaxCapacity, @CurrentBookings)";
        db.Connection.Execute(sql, session);
    }
}