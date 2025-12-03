using System.Collections.Generic;
using Dapper;

public class ResMenuAccess
{
    private readonly DatabaseContext _db;

    public ResMenuAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Link(ResMenuModel resMenu)
    {
        const string sql = @"INSERT INTO ResMenu (ReservationID, MenuID)
                             VALUES (@ReservationID, @MenuID)";
        _db.Connection.Execute(sql, resMenu);
    }

    public IEnumerable<ResMenuModel> GetAll()
    {
        const string sql = "SELECT * FROM ResMenu";
        return _db.Connection.Query<ResMenuModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM ResMenu WHERE ID = @ID";
        _db.Connection.Execute(sql, new { ID = id });
    }
}
