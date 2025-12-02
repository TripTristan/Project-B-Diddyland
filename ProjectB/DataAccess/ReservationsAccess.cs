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
}
