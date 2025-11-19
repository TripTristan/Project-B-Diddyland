using Microsoft.Data.Sqlite;
using Dapper;

public static class TicketAttractieAccess
{
    public static void Link(TicketAttractieModel ta)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO TicketAttractie (TicketID, AttractieID)
                                 VALUES (@TicketID, @AttractieID)";
            DBC.Connection.Execute(sql, ta);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<TicketAttractieModel> GetAll()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM TicketAttractie";
            return DBC.Connection.Query<TicketAttractieModel>(sql);
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

            const string sql = "DELETE FROM TicketAttractie WHERE ID = @ID";
            DBC.Connection.Execute(sql, new { ID = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}
