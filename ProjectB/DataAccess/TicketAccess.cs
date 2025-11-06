using Microsoft.Data.Sqlite;
using Dapper;

public class TicketsAccess
{

    public static void Insert(TicketModel ticket)
    {
        const string sql = @"INSERT INTO Ticket (Type, Price, ReservationID)
                             VALUES (@Type, @Price, @ReservationID)";
        DBC.Connection.Execute(sql, ticket);
    }

    public static IEnumerable<TicketModel> GetAll()
    {
        const string sql = "SELECT * FROM Ticket";
        return DBC.Connection.Query<TicketModel>(sql);
    }

    public static TicketModel? GetById(int ticketId)
    {
        const string sql = "SELECT * FROM Ticket WHERE TicketID = @TicketID";
        return DBC.Connection.QueryFirstOrDefault<TicketModel>(sql, new { TicketID = ticketId });
    }

    public static void Delete(int ticketId)
    {
        const string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
        DBC.Connection.Execute(sql, new { TicketID = ticketId });
    }
}
