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
                 Currentbookings AS CurrentBookings
              FROM Sessions").ToList();

    public Session? GetSessionById(int id)
        => _db.Connection.QueryFirstOrDefault<Session>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 AttractionID,
                 Currentbookings AS CurrentBookings
              FROM Sessions
              WHERE ID = @Id", new { Id = id });

    public void UpdateSession(Session session)
    {
        const string sql = @"UPDATE Sessions
                             SET Currentbookings = @CurrentBookings
                             WHERE ID = @Id";
        _db.Connection.Execute(sql, session);
    }

    public void Insert(Session session)
    {
        const string sql = @"INSERT INTO Sessions
                             (Date, Time, AttractionID, Currentbookings)
                             VALUES (@Date, @Time, @AttractionID, @CurrentBookings)";
        _db.Connection.Execute(sql, session);
    }

    public int GetCapacityBySession(Session sesh)
    {
<<<<<<< HEAD
        var attr = AttractionAccess.GetById(sesh.AttractionID);
=======
        var attr = _attractiesAccess.GetById(sesh.AttractionID);
>>>>>>> main
        return attr?.Capacity ?? 0;
    }

    public List<Session> GetSessionsForAttractionOnDate(int attractionId, DateTime date)
    {
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

        return _db.Connection.Query<Session>(sql, new { AttractionID = attractionId, Date = day }).ToList();
    }

    public List<Session> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date);
        if (existing.Count > 0) return existing;

<<<<<<< HEAD
        _ = AttractionAccess.GetById(attractionId)
=======
        _ = _attractiesAccess.GetById(attractionId)
>>>>>>> main
            ?? throw new InvalidOperationException($"Attraction {attractionId} not found.");

        const string insertSql = @"INSERT INTO Sessions
                                   (Date, Time, AttractionID, Currentbookings)
                                   VALUES (@Date, @Time, @AttractionID, 0)";

        foreach (var t in GenerateHalfHourSlots())
        {
            _db.Connection.Execute(insertSql, new
            {
                Date = date.ToString("yyyy-MM-dd"),
                Time = t,
                AttractionID = attractionId
            });
        }

        return GetSessionsForAttractionOnDate(attractionId, date);
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
