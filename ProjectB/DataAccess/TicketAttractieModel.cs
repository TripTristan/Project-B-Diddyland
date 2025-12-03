using System.Collections.Generic;
using Dapper;

public class TicketAttractieAccess
{
    private readonly DatabaseContext _db;

    public TicketAttractieAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Link(TicketAttractieModel ta)
    {
        const string sql = @"INSERT INTO TicketAttractie (TicketID, AttractieID)
                             VALUES (@TicketID, @AttractieID)";
        _db.Connection.Execute(sql, ta);
    }

    public IEnumerable<TicketAttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM TicketAttractie";
        return _db.Connection.Query<TicketAttractieModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM TicketAttractie WHERE ID = @ID";
        _db.Connection.Execute(sql, new { ID = id });
    }
}
