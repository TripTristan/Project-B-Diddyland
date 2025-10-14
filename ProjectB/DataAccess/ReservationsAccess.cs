using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;

public class ReservationsAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/project.db");

    public void AddBooking(Booking booking)
    {
        const string sql =
            @"INSERT INTO Bookings (OrderNumber, SessionId, BookingDate, Period, CreatedAt)
              VALUES (@OrderNumber, @SessionId, @BookingDate, @Period, @CreatedAt)";
        _connection.Execute(sql, booking);

        Console.WriteLine($"[Debug] Booking added: {booking.OrderNumber}");
    }

    public List<Booking> GetAllBookings()
    {
        return _connection.Query<Booking>("SELECT * FROM Bookings").ToList();
    }

}