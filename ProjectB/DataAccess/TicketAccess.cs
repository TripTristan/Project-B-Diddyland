using Microsoft.Data.Sqlite;
using Dapper;

public static class TicketsAccess
{

    public static void Insert(TicketModel ticket)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO Ticket (Type, Price, ReservationID)
                                 VALUES (@Type, @Price, @ReservationID)";
            DBC.Connection.Execute(sql, ticket);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<TicketModel> GetAll()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Ticket";
            return DBC.Connection.Query<TicketModel>(sql);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static TicketModel? GetById(int ticketId)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Ticket WHERE TicketID = @TicketID";
            return DBC.Connection.QueryFirstOrDefault<TicketModel>(sql, new { TicketID = ticketId });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Delete(int ticketId)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
            DBC.Connection.Execute(sql, new { TicketID = ticketId });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}
