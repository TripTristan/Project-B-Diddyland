using Microsoft.Data.Sqlite;
using Dapper;

public class MenusAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland.db");

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

    public MenuModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Menu WHERE ID = @ID";
        return _connection.QueryFirstOrDefault<MenuModel>(sql, new { ID = id });
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Menu WHERE ID = @ID";
        _connection.Execute(sql, new { ID = id });
    }
}
