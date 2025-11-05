using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

public static class SessionAccess
{

    public static List<Session> GetAllSessions()
    {
        return DBC.Connection.Query<Session>("SELECT * FROM Sessions").ToList();
    }

    public static Session? GetSessionById(int id)
    {
        return DBC.Connection.QueryFirstOrDefault<Session>(
            "SELECT * FROM Sessions WHERE Id = @Id", new { Id = id });
    }

    public static void UpdateSession(Session session)
    {
        DBC.Connection.Execute(
            @"UPDATE Sessions
              SET CurrentBookings = @CurrentBookings
              WHERE Id = @Id",
            session);
    }
    public static void Insert(Session session)
    {
        const string sql = @"INSERT INTO Sessions (Date, Time, MaxCapacity, CurrentBookings)
                             VALUES (@Date, @Time, @MaxCapacity, @CurrentBookings)";
        DBC.Connection.Execute(sql, session);
    }
    
    public static int GetCapacityBySession(Session sesh)
    {
        return AttractiesAccess.GetById(sesh.AttractionID).Capacity;
    }
}