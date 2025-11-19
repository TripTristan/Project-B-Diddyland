using Microsoft.Data.Sqlite;
using Dapper;

public static class MenusAccess
{
    
    public static void Insert(MenuModel menu)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"INSERT INTO Menu (Food, Drink, Price)
                                 VALUES (@Food, @Drink, @Price)";
            DBC.Connection.Execute(sql, menu);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<MenuModel> GetAll()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Menu";
            return DBC.Connection.Query<MenuModel>(sql);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static MenuModel? GetById(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT * FROM Menu WHERE ID = @ID";
            return DBC.Connection.QueryFirstOrDefault<MenuModel>(sql, new { ID = id });
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

            const string sql = "DELETE FROM Menu WHERE ID = @ID";
            DBC.Connection.Execute(sql, new { ID = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}
