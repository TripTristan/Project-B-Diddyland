using System;
using System.Collections.Generic;
using Dapper;

public class ReservationAccess
{
    private readonly DatabaseContext _db;
    private readonly UserAccess _userAccess;
    private readonly List<ReservationModel> _bookings = new();
    public const string Table = "Bookings";


    public ReservationAccess(DatabaseContext db, UserAccess userAccess)
    {
        _db = db;
        _userAccess = userAccess;
        _db.Dispose();
    }


    public List<ReservationModel> GetAllBookingsByUserID(int id)
    {
        string sql = $@"
SELECT OrderNumber, SessionId, Quantity, BookingDate, Price, CustomerId, Type
FROM {Table}
WHERE CustomerId = @Id; ";

        return _db.Connection.Query<ReservationModel>(sql, new { Id = id }).ToList();
    }

    public List<ReservationModel> GetAllOrdersBetweenDates(long date1, long date2)
    {
        string sql = $@"SELECT OrderNumber, SessionId, Quantity, BookingDate, Price, CustomerId, Type FROM {Table} WHERE BookingDate BETWEEN {date1} AND {date2};";

        return _db.Connection.Query<ReservationModel>(sql).ToList();
    }



    public void AddBooking(ReservationModel booking)
    {
        string sql =
            $"INSERT INTO {Table} " +
            "(OrderNumber, SessionID, Quantity, CustomerId, BookingDate, Price, Type) " +
            "VALUES (@OrderNumber, @SessionId, @Quantity, @CustomerID, @BookingDate, @Price, @FastPass);";

        _db.Connection.Execute(sql, booking);

        var name = _userAccess.GetNameById(booking.CustomerID) ?? "Guest";

        _bookings.Add(booking);
    }

    public int CountDistinctVisitDates(int userId)
    {
        return _db.Connection.ExecuteScalar<int>(
            @"SELECT COUNT(DISTINCT s.Date)
            FROM Bookings b
            JOIN Sessions s ON s.Id = b.SessionId
            WHERE b.CustomerId = @UserId",
            new { UserId = userId }
        );
    }



    

}
