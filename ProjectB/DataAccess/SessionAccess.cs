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
                 Capacity,
                 AttractionId,
                 Location,
                 SessionType
              FROM Sessions").ToList();

    public SessionModel? GetSessionById(long id)
        => _db.Connection.QueryFirstOrDefault<SessionModel>(
            @"SELECT 
                 ID AS Id,
                 Date,
                 Time,
                 Capacity,
                 AttractionId,
                 Location,
                 SessionType
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
        const string sql = @"INSERT INTO Sessions
                             (ID, Date, Time, Capacity, AttractionId, Location, SessionType)
                             VALUES (@Id, @Date, @Time, @Capacity, @AttractionId, @Location, @SessionType);";

        _db.Connection.Execute(sql, session);
    }

    public List<SessionModel> GetNormalSessionsForAttractionOnDate(int attractionId, DateTime date, string location)
    {
        long dateTicks = date.Date.Ticks;

        const string sql = @"SELECT
                                ID AS Id,
                                Date,
                                Time,
                                Capacity,
                                AttractionId,
                                Location,
                                SessionType
                             FROM Sessions
                             WHERE Date = @Date
                               AND AttractionId = @AttractionId
                               AND Location = @Location
                               AND SessionType = 0
                             ORDER BY Time";

        return _db.Connection.Query<SessionModel>(sql, new
        {
            Date = dateTicks,
            AttractionId = attractionId,
            Location = location
        }).ToList();
    }

    public List<SessionModel> EnsureNormalSessionsForAttractionAndDate(int attractionId, DateTime date, string location)
    {
        var existing = GetNormalSessionsForAttractionOnDate(attractionId, date, location);
        if (existing.Any())
            return existing;

        int cap = GetAttractionCapacity(attractionId, fallback: 35);

        var newSessions = new List<SessionModel>();

        for (int i = 1; i <= 3; i++)
        {
            var session = new SessionModel(NextId(), date.Date.Ticks, i, cap)
            {
                AttractionId = attractionId,
                Location = location,
                SessionType = 0
            };

            Insert(session);
            newSessions.Add(session);
        }

        return newSessions;
    }

    public List<SessionModel> GetFastPassSessionsForAttractionOnDate(int attractionId, DateTime date, string location)
    {
        long dateTicks = date.Date.Ticks;

        const string sql = @"SELECT
                                ID AS Id,
                                Date,
                                Time,
                                Capacity,
                                AttractionId,
                                Location,
                                SessionType
                             FROM Sessions
                             WHERE Date = @Date
                               AND AttractionId = @AttractionId
                               AND Location = @Location
                               AND SessionType = 1
                             ORDER BY Time";

        return _db.Connection.Query<SessionModel>(sql, new
        {
            Date = dateTicks,
            AttractionId = attractionId,
            Location = location
        }).ToList();
    }

    public List<SessionModel> EnsureFastPassSessionsForAttractionAndDate(int attractionId, DateTime date, string location)
    {
        var existing = GetFastPassSessionsForAttractionOnDate(attractionId, date, location);
        if (existing.Any())
            return existing;

        int cap = GetAttractionCapacity(attractionId, fallback: 10);

        var created = new List<SessionModel>();
        var slots = GenerateHalfHourSlots(); 

        foreach (var slotTicks in slots)
        {
            var session = new SessionModel(NextId(), date.Date.Ticks, slotTicks, cap)
            {
                AttractionId = attractionId,
                Location = location,
                SessionType = 1
            };

            Insert(session);
            created.Add(session);
        }

        return created;
    }

    private int GetAttractionCapacity(int attractionId, int fallback)
    {
        try
        {
            var attraction = _attractiesAccess.GetById(attractionId);
            if (attraction != null && attraction.Capacity > 0)
                return attraction.Capacity;
        }
        catch
        {
        }

        return fallback;
    }

    private List<long> GenerateHalfHourSlots()
    {
        var slots = new List<long>();
        var start = new TimeSpan(9, 0, 0);
        var end = new TimeSpan(21, 0, 0);

        for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
            slots.Add(t.Ticks);

        return slots;
    }

    public List<SessionModel> GetAllSessionsForDate(long date)
    {
        const string sql = @"SELECT
                                ID AS Id,
                                Date,
                                Time,
                                Capacity,
                                AttractionId,
                                Location,
                                SessionType
                             FROM Sessions
                             WHERE Date = @Date";
        return _db.Connection.Query<SessionModel>(sql, new { Date = date }).ToList();
    }

    public int NextId()
    {
        try
        {
            const string sql = @"SELECT IFNULL(MAX(Id), 0) + 1 FROM Sessions";
            return _db.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
    }
}
