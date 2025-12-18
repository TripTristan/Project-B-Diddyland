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
        long dateTicks = date.Date.Ticks;
        const string sql = @"SELECT 
                                ID AS Id,
                                Date,
                                Time,
                                Capacity
                             FROM Sessions
                             WHERE Date = @Date
                             ORDER BY Time";

        return _db.Connection.Query<SessionModel>(sql, new { Date = dateTicks }).ToList();
    }

    public List<SessionModel> GetSessionsForAttractionOnDate(int attractionId, DateTime date, string location)
    {
        long dateTicks = date.Date.Ticks;
        const string sql = @"SELECT 
                                ID AS Id,
                                Date,
                                Time,
                                Capacity
                             FROM Sessions
                             WHERE Date = @Date
                             ORDER BY Time";

        return _db.Connection.Query<SessionModel>(sql, new { Date = dateTicks }).ToList();
    }

    public List<SessionModel> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date);
        if (existing.Any())
            return existing;

        var newSessions = new List<SessionModel>();

        for (int i = 1; i <= 3; i++)
        {
            var session = new SessionModel(NextId(), date.Ticks, i, 35);
            Insert(session);
            newSessions.Add(session);
        }

        return newSessions;
    }
    public List<SessionModel> EnsureSessionsForAttractionAndDate(int attractionId, DateTime date, string location)
    {
        var existing = GetSessionsForAttractionOnDate(attractionId, date, location);
        if (existing.Any())
            return existing;

        var newSessions = new List<SessionModel>();

        for (int i = 1; i <= 3; i++)
        {
            var session = new SessionModel(NextId(), date.Ticks, i, 35);
            Insert(session);
            newSessions.Add(session);
        }

        return newSessions;
    }

    private List<long> GenerateHalfHourSlots()
    {
        var slots = new List<long>();
        var start = new TimeSpan(9, 0, 0);
        var end = new TimeSpan(21, 0, 0);

        for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
        {
            slots.Add(t.Ticks);
        }

        return slots;
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
