using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

public static class SessionAccess
{
    public static List<Session> GetAllSessions()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            return DBC.Connection.Query<Session>(
                @"SELECT 
                     ID AS Id,
                     Date,
                     Time,
                     AttractionID,
                     Currentbookings AS CurrentBookings
                  FROM Sessions").ToList();
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static Session? GetSessionById(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            return DBC.Connection.QueryFirstOrDefault<Session>(
                @"SELECT 
                     ID AS Id,
                     Date,
                     Time,
                     AttractionID,
                     Currentbookings AS CurrentBookings
                  FROM Sessions
                  WHERE ID = @Id", new { Id = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void UpdateSession(Session session)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"UPDATE Sessions
                                 SET Currentbookings = @CurrentBookings
                                 WHERE ID = @Id";
            DBC.Connection.Execute(sql, session);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Insert(Session session)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO Sessions
                                 (Date, Time, AttractionID, Currentbookings)
                                 VALUES (@Date, @Time, @AttractionID, @CurrentBookings)";
            DBC.Connection.Execute(sql, session);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static int GetCapacityBySession(Session sesh)
    {
        var attr = AttractiesAccess.GetById(sesh.AttractionID);
        return attr?.Capacity ?? 0;
    }

    public static List<Session> GetSessionsForAttractionOnDate(int attractionId, DateTime date)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string day = date.ToString("yyyy-MM-dd");
            const string sql = @"SELECT 
                                    ID AS Id,
                                    Date,
                                    Time,
                                    AttractionID,
                                    Currentbookings AS CurrentBookings
                                 FROM Sessions
                                 WHERE AttractionID = @AttractionID AND Date = @Date
                                 ORDER BY Time";
            return DBC.Connection.Query<Session>(sql, new { AttractionID = attractionId, Date = day }).ToList();
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static List<Session> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date);
        if (existing.Count > 0) return existing;

        _ = AttractiesAccess.GetById(attractionId)
            ?? throw new InvalidOperationException($"Attraction {attractionId} not found.");

        const string insertSql = @"INSERT INTO Sessions
                                   (Date, Time, AttractionID, Currentbookings)
                                   VALUES (@Date, @Time, @AttractionID, 0)";

        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            foreach (var t in GenerateHalfHourSlots())
            {
                DBC.Connection.Execute(insertSql, new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Time = t,              
                    AttractionID = attractionId
                });
            }
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }

        return GetSessionsForAttractionOnDate(attractionId, date);
    }

    private static IEnumerable<string> GenerateHalfHourSlots()
    {
        var start = new TimeSpan(9, 0, 0);
        var end = new TimeSpan(18, 0, 0);
        for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
            yield return new DateTime(1, 1, 1, t.Hours, t.Minutes, 0).ToString("HH:mm");
    }
}
