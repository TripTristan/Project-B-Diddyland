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

    public List<SessionModel> GetAllSessions()
        => _db.Connection.Query<SessionModel>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 Capacity
              FROM Sessions").ToList();

    public SessionModel? GetSessionById(long id)
        => _db.Connection.QueryFirstOrDefault<SessionModel>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 Capacity
              FROM Sessions
              WHERE ID = @Id", new { Id = id });

    public void UpdateSession(SessionModel session)
    {
        const string sql = @"UPDATE Sessions
                             SET Capacity = @Capacity
                             WHERE ID = @Id";
        _db.Connection.Execute(sql, session);
    }

    public void Insert(SessionModel session)
    {
        Console.WriteLine($"{session.Id} {session.Time} {session.Date} {session.Capacity}");
        string sql = @"INSERT INTO Sessions
                             (ID, Date, Time, Capacity)
                             VALUES (@Id, @Date, @Time, @Capacity);";
        _db.Connection.Execute(sql, session);
    }

    public List<SessionModel> GetSessionsForAttractionOnDate(int attractionId, DateTime date)
    {
        string day = date.ToString("yyyy-MM-dd");
        const string sql = @"SELECT 
                                ID AS Id,
                                Date,
                                Time,
                                Capacity
                             FROM Sessions
                             Date = @Date
                             ORDER BY Time";

        return _db.Connection.Query<SessionModel>(sql, new { AttractionID = attractionId, Date = day }).ToList();
    }

    public List<SessionModel> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date);
        if (existing.Count > 0) return existing;

        _ = _attractiesAccess.GetById(attractionId)
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

    public List<SessionModel> GetAllSessionsForDate(long date)
    {
        string sql = $@"SELECT * FROM Sessions WHERE Date = {date}";
        return _db.Connection.Query<SessionModel>(sql).ToList();
    }

    public int NextId()
    {
        try
        {
            string sql = $"SELECT IFNULL(MAX(Id), 0) + 1 FROM Sessions";
            return _db.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
    }

}
