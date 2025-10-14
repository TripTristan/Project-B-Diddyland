using Microsoft.Data.Sqlite;
using Dapper;

public class ResMenuAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland.db");

    public void Link(ResMenuModel resMenu)
    {
        const string sql = @"INSERT INTO ResMenu (ReservationID, MenuID)
                             VALUES (@ReservationID, @MenuID)";
        _connection.Execute(sql, resMenu);
    }

    public IEnumerable<ResMenuModel> GetAll()
    {
        const string sql = "SELECT * FROM ResMenu";
        return _connection.Query<ResMenuModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM ResMenu WHERE ID = @ID";
        _connection.Execute(sql, new { ID = id });
    }
}
