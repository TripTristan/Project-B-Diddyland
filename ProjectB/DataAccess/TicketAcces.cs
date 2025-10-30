using Microsoft.Data.Sqlite;
using Dapper;

public class TicketsAccess
{
    DBC db = new();

    public void Insert(TicketModel ticket)
    {
        const string sql = @"INSERT INTO Ticket (Type, Price, ReservationID)
                             VALUES (@Type, @Price, @ReservationID)";
        db.Connection.Execute(sql, ticket);
    }

    public IEnumerable<TicketModel> GetAll()
    {
        const string sql = "SELECT * FROM Ticket";
        return db.Connection.Query<TicketModel>(sql);
    }

    public TicketModel? GetById(int ticketId)
    {
        const string sql = "SELECT * FROM Ticket WHERE TicketID = @TicketID";
        return db.Connection.QueryFirstOrDefault<TicketModel>(sql, new { TicketID = ticketId });
    }

    public void Delete(int ticketId)
    {
        const string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
        db.Connection.Execute(sql, new { TicketID = ticketId });
    }
}
