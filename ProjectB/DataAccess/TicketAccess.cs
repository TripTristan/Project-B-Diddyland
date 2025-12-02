using System.Collections.Generic;
using Dapper;

public class TicketsAccess
{
    private readonly DatabaseContext _db;

    public TicketsAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Insert(TicketModel ticket)
    {
        const string sql = @"INSERT INTO Ticket (Type, Price, ReservationID)
                             VALUES (@Type, @Price, @ReservationID)";
        _db.Connection.Execute(sql, ticket);
    }

    public IEnumerable<TicketModel> GetAll()
    {
        const string sql = "SELECT * FROM Ticket";
        return _db.Connection.Query<TicketModel>(sql);
    }

    public TicketModel? GetById(int ticketId)
    {
        const string sql = "SELECT * FROM Ticket WHERE TicketID = @TicketID";
        return _db.Connection.QueryFirstOrDefault<TicketModel>(sql, new { TicketID = ticketId });
    }

    public void Delete(int ticketId)
    {
        const string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
        _db.Connection.Execute(sql, new { TicketID = ticketId });
    }
}
