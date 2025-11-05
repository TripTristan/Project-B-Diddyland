using Microsoft.Data.Sqlite;
using Dapper;

public class ResMenuAccess
{
    public static void Link(ResMenuModel resMenu)
    {
        const string sql = @"INSERT INTO ResMenu (ReservationID, MenuID)
                             VALUES (@ReservationID, @MenuID)";
        DBC.Connection.Execute(sql, resMenu);
    }

    public static IEnumerable<ResMenuModel> GetAll()
    {
        const string sql = "SELECT * FROM ResMenu";
        return DBC.Connection.Query<ResMenuModel>(sql);
    }

    public static void Delete(int id)
    {
        const string sql = "DELETE FROM ResMenu WHERE ID = @ID";
        DBC.Connection.Execute(sql, new { ID = id });
    }
}
