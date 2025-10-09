using Microsoft.Data.Sqlite;
using Dapper;

public class MenusAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");

    public void Insert(MenuModel menu)
    {
        const string sql = @"INSERT INTO Menu (Food, Drink, Price)
                             VALUES (@Food, @Drink, @Price)";
        _connection.Execute(sql, menu);
    }

    public IEnumerable<MenuModel> GetAll()
    {
        const string sql = "SELECT * FROM Menu";
        return _connection.Query<MenuModel>(sql);
    }

    public MenuModel? GetById(int menuId)
    {
        const string sql = "SELECT * FROM Menu WHERE MenuID = @MenuID";
        return _connection.QueryFirstOrDefault<MenuModel>(sql, new { MenuID = menuId });
    }

    public void Delete(int menuId)
    {
        const string sql = "DELETE FROM Menu WHERE MenuID = @MenuID";
        _connection.Execute(sql, new { MenuID = menuId });
    }
}
