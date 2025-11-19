using Microsoft.Data.Sqlite;
using Dapper;

public static class ResMenuAccess
{
    public static void Link(ResMenuModel resMenu)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO ResMenu (ReservationID, MenuID)
                                 VALUES (@ReservationID, @MenuID)";
            DBC.Connection.Execute(sql, resMenu);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<ResMenuModel> GetAll()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM ResMenu";
            return DBC.Connection.Query<ResMenuModel>(sql);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Delete(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "DELETE FROM ResMenu WHERE ID = @ID";
            DBC.Connection.Execute(sql, new { ID = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}
