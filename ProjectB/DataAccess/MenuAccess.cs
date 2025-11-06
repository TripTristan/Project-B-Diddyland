using Microsoft.Data.Sqlite;
using Dapper;

public static class MenusAccess
{
    
    public static void Insert(MenuModel menu)
    {
        const string sql = @"INSERT INTO Menu (Food, Drink, Price)
                             VALUES (@Food, @Drink, @Price)";
        DBC.Connection.Execute(sql, menu);
    }

    public static IEnumerable<MenuModel> GetAll()
    {
        const string sql = "SELECT * FROM Menu";
        return DBC.Connection.Query<MenuModel>(sql);
    }

    public static MenuModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Menu WHERE ID = @ID";
        return DBC.Connection.QueryFirstOrDefault<MenuModel>(sql, new { ID = id });
    }

    public static void Delete(int id)
    {
        const string sql = "DELETE FROM Menu WHERE ID = @ID";
        DBC.Connection.Execute(sql, new { ID = id });
    }
}
