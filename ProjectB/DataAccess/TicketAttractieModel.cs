using Microsoft.Data.Sqlite;
using Dapper;

public class TicketAttractieAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");

    public void Link(TicketAttractieModel ta)
    {
        const string sql = @"INSERT INTO TicketAttractie (TicketID, AttractieID)
                             VALUES (@TicketID, @AttractieID)";
        _connection.Execute(sql, ta);
    }

    public IEnumerable<TicketAttractieModel> GetAll()
    {
        const string sql = "SELECT * FROM TicketAttractie";
        return _connection.Query<TicketAttractieModel>(sql);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM TicketAttractie WHERE ID = @ID";
        _connection.Execute(sql, new { ID = id });
    }
}
