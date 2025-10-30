using Microsoft.Data.Sqlite;
using Dapper;

public class TicketAttractieAccess
{
    DBC db = new();

    public void Link(TicketAttractieModel ta)
    {
        const string sql = @"INSERT INTO TicketAttractie (TicketID, AttractieID)
                             VALUES (@TicketID, @AttractieID)";
        db.Connection.Execute(sql, ta);
    }

    public IEnumerable<TicketAttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM TicketAttractie";
        return db.Connection.Query<TicketAttractieModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM TicketAttractie WHERE ID = @ID";
        db.Connection.Execute(sql, new { ID = id });
    }
}
