using System.Collections.Generic;
using Dapper;

public class MenusAccess
{
    private readonly DatabaseContext _db;

    public MenusAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Insert(MenuModel menu)
    {
        const string sql = @"INSERT INTO Menu (Food, Drink, Price)
                             VALUES (@Food, @Drink, @Price)";
        _db.Connection.Execute(sql, menu);
    }

    public IEnumerable<MenuModel> GetAll()
    {
        const string sql = "SELECT * FROM Menu";
        return _db.Connection.Query<MenuModel>(sql);
    }

    public MenuModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Menu WHERE ID = @ID";
        return _db.Connection.QueryFirstOrDefault<MenuModel>(sql, new { ID = id });
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Menu WHERE ID = @ID";
        _db.Connection.Execute(sql, new { ID = id });
    }
}
