using Microsoft.Data.Sqlite;
using Dapper;

public class MenusAccess
{
    
    DBC db = new();
    public void Insert(MenuModel menu)
    {
        const string sql = @"INSERT INTO Menu (Food, Drink, Price)
                             VALUES (@Food, @Drink, @Price)";
        db.Connection.Execute(sql, menu);
    }

    public IEnumerable<MenuModel> GetAll()
    {
        const string sql = "SELECT * FROM Menu";
        return db.Connection.Query<MenuModel>(sql);
    }

    public MenuModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Menu WHERE ID = @ID";
        return db.Connection.QueryFirstOrDefault<MenuModel>(sql, new { ID = id });
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Menu WHERE ID = @ID";
        db.Connection.Execute(sql, new { ID = id });
    }
}
