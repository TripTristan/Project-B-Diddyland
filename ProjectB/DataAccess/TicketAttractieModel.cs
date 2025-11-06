using Microsoft.Data.Sqlite;
using Dapper;

public class TicketAttractieAccess
{

    public static void Link(TicketAttractieModel ta)
    {
        const string sql = @"INSERT INTO TicketAttractie (TicketID, AttractieID)
                             VALUES (@TicketID, @AttractieID)";
        DBC.Connection.Execute(sql, ta);
    }

    public static IEnumerable<TicketAttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM TicketAttractie";
        return DBC.Connection.Query<TicketAttractieModel>(sql);
    }

    public static void Delete(int id)
    {
        const string sql = "DELETE FROM TicketAttractie WHERE ID = @ID";
        DBC.Connection.Execute(sql, new { ID = id });
    }
}
