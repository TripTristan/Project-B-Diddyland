using Microsoft.Data.Sqlite;
using Dapper;

public class TicketsAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");

    public void Insert(TicketModel ticket)
    {
        const string sql = @"INSERT INTO Ticket (Type, Price, ReservationID)
                             VALUES (@Type, @Price, @ReservationID)";
        _connection.Execute(sql, ticket);
    }

    public IEnumerable<TicketModel> GetAll()
    {
        const string sql = "SELECT * FROM Ticket";
        return _connection.Query<TicketModel>(sql);
    }

    public TicketModel? GetById(int ticketId)
    {
        const string sql = "SELECT * FROM Ticket WHERE TicketID = @TicketID";
        return _connection.QueryFirstOrDefault<TicketModel>(sql, new { TicketID = ticketId });
    }

    public void Delete(int ticketId)
    {
        const string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
        _connection.Execute(sql, new { TicketID = ticketId });
    }
}
