using Microsoft.Data.Sqlite;
using Dapper;

public class ReservationsAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland.db");

    public void Insert(ReservationModel reservation)
    {
        const string sql = @"INSERT INTO Reservation (AccountID, TicketID, Price, Date, Amount)
                             VALUES (@AccountID, @TicketID, @Price, @Date, @Amount)";
        _connection.Execute(sql, reservation);
    }

    public IEnumerable<ReservationModel> GetByUser(int accountId)
    {
        const string sql = "SELECT * FROM Reservation WHERE AccountID = @AccountID";
        return _connection.Query<ReservationModel>(sql, new { AccountID = accountId });
    }

    public IEnumerable<ReservationModel> GetAll()
    {
        const string sql = "SELECT * FROM Reservation";
        return _connection.Query<ReservationModel>(sql);
    }

    public void Delete(int reservationId)
    {
        const string sql = "DELETE FROM Reservation WHERE ReservationID = @ReservationID";
        _connection.Execute(sql, new { ReservationID = reservationId });
    }
}
