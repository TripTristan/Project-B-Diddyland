public class AdminSessionLogic
{
    private readonly SessionAccess _sessionAccess;

    public AdminSessionLogic(SessionAccess sessionAccess)
    {
        _sessionAccess = sessionAccess;
    }

    public List<SessionModel> GetSessionsByDate(DateTime date)
        => _sessionAccess.GetAllSessionsForDate(date.Ticks);

    public void SetCapacity(long sessionId, long newCapacity)
    {
        var s = _sessionAccess.GetSessionById(sessionId) ?? throw new Exception("Session not found.");
        s.Capacity = newCapacity;
        _sessionAccess.UpdateSession(s);
    }
}
