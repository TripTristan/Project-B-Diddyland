using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

public class SessionAccess
{
    private readonly DatabaseContext _db;
    private readonly AttractiesAccess _attractiesAccess;

    public SessionAccess(DatabaseContext db, AttractiesAccess attractiesAccess)
    {
        _db = db;
        _attractiesAccess = attractiesAccess;
    }

    public List<Session> GetAllSessions()
        => _db.Connection.Query<Session>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 AttractionID,
                 Currentbookings AS CurrentBookings,
                 Location
              FROM Sessions").ToList();

    public Session? GetSessionById(int id)
        => _db.Connection.QueryFirstOrDefault<Session>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 AttractionID,
                 Currentbookings AS CurrentBookings,
                 Location
              FROM Sessions
              WHERE ID = @Id", new { Id = id });

    public void UpdateSession(Session session)
    {
        const string sql = @"UPDATE Sessions
                             SET Currentbookings = @CurrentBookings
                             WHERE ID = @Id";
        _db.Connection.Execute(sql, session);
    }

    public int Insert(Session session)
    {
        const string sql = @"INSERT INTO Sessions
                             (Date, Time, AttractionID, Currentbookings, Location)
                             VALUES (@Date, @Time, @AttractionID, @CurrentBookings, @Location);
                             SELECT last_insert_rowid()";
        int newId = _db.Connection.QuerySingle<int>(sql, session);
        session.Id = newId;
        return newId;
    }

    public int GetCapacityBySession(Session sesh)
    {
        var attr = _attractiesAccess.GetById(sesh.AttractionID);
        return attr?.Capacity ?? 0;
    }

    public List<Session> GetSessionsForAttractionOnDate(int attractionId, DateTime date, string location)
    {
        string day = date.ToString("yyyy-MM-dd");

        const string sql = @"SELECT 
                                ID AS Id,
                                Date,
                                Time,
                                AttractionID,
                                Currentbookings AS CurrentBookings,
                                Location
                             FROM Sessions
                             WHERE AttractionID = @AttractionID 
                               AND Date = @Date
                               AND Location = @Location
                             ORDER BY Time";

        return _db.Connection.Query<Session>(sql, new
        {
            AttractionID = attractionId,
            Date = day,
            Location = location
        }).ToList();
    }

    public List<Session> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date, string location)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date, location);
        if (existing.Any())
            return existing;

        var newSessions = new List<Session>();
        string day = date.ToString("yyyy-MM-dd");

        foreach (var time in GenerateHalfHourSlots())
        {
            var session = new Session
            {
                Date = day,
                Time = time,
                AttractionID = attractionId,
                CurrentBookings = 0,
                Location = location
            };

            Insert(session);
            newSessions.Add(session);
        }

        return newSessions;
    }

    private IEnumerable<string> GenerateHalfHourSlots()
    {
        var start = new TimeSpan(9, 0, 0);
        var end = new TimeSpan(18, 0, 0);

        for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
        {
            yield return new DateTime(1, 1, 1, t.Hours, t.Minutes, 0).ToString("HH:mm");
        }
    }
}
