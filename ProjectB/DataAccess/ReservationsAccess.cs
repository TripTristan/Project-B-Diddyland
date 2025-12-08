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
<<<<<<< HEAD
        string sql = $"INSERT INTO {Table} (OrderNumber, SessionID, Quantity, BookingDate, OriginalPrice, Discount, FinalPrice, CustomerId) VALUES (@OrderNumber, @SessionId, @Quantity, @BookingDate, @OriginalPrice, @Discount, @FinalPrice, @CustomerID);";
        DBC.Connection.Execute(sql, booking);
        Console.WriteLine($"[DB] Added ticket for {UserAccess.GetNameById(booking.CustomerID) ?? "Guest"} ({booking.OrderNumber})");
        DBC.CloseConnection();
    }

    public static List<ReservationModel> GetAllBookings() => _bookings;

    public static List<ReservationModel> GetAllBookingsByUserID(int id)
    {
        string sql = $@"
SELECT OrderNumber, SessionId, Quantity, BookingDate, OriginalPrice * 1.0 AS OriginalPrice, Discount * 1.0 AS Discount, FinalPrice * 1.0 AS FinalPrice, CustomerId 
FROM {Table}
WHERE CustomerId = @Id; ";

        return DBC.Connection.Query<ReservationModel>(sql, new { Id = id }).ToList();
    }

    public static List<ReservationModel> GetAllOrdersBetweenDates(long date1, long date2)
    {
        string sql = $@"SELECT OrderNumber, SessionId, Quantity, BookingDate, OriginalPrice * 1.0 AS OriginalPrice, Discount * 1.0 AS Discount, FinalPrice * 1.0 AS FinalPrice, CustomerId FROM {Table} WHERE BookingDate BETWEEN {date1} AND {date2};";

        return DBC.Connection.Query<ReservationModel>(sql).ToList();
    }


    
    
=======
        _db = db;
        _userAccess = userAccess;
    }

    public void AddBooking(ReservationModel booking)
    {
        string sql =
            $"INSERT INTO {Table} " +
            "(OrderNumber, SessionID, Quantity, CustomerId, BookingDate, OriginalPrice, Discount, FinalPrice, Type) " +
            "VALUES (@OrderNumber, @SessionId, @Quantity, @CustomerID, @BookingDate, @OriginalPrice, @Discount, @FinalPrice, @Type);";

        _db.Connection.Execute(sql, booking);

        var name = _userAccess.GetNameById(booking.CustomerID) ?? "Guest";
        Console.WriteLine($"[DB] Added ticket for {name} ({booking.OrderNumber})");

        _bookings.Add(booking);
    }

    public List<ReservationModel> GetAllBookings() => _bookings;
>>>>>>> main
}
