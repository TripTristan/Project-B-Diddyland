using Microsoft.Data.Sqlite;
using Dapper;

public class ResMenuAccess
{
    DBC db = new();

    public void Link(ResMenuModel resMenu)
    {
        const string sql = @"INSERT INTO ResMenu (ReservationID, MenuID)
                             VALUES (@ReservationID, @MenuID)";
        db.Connection.Execute(sql, resMenu);
    }

    public IEnumerable<ResMenuModel> GetAll()
    {
        const string sql = "SELECT * FROM ResMenu";
        return db.Connection.Query<ResMenuModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM ResMenu WHERE ID = @ID";
        db.Connection.Execute(sql, new { ID = id });
    }
}
