using Dapper;
using Microsoft.Data.Sqlite;

public class ReservationsServiceAccess
{
    private readonly List<GroupReservationModel> _bookings = new();
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland2.db");

    public void AddBooking(GroupReservationModel booking)
    {
        _bookings.Add(booking);

        const string sql = @"
            INSERT INTO GroupReservations
            (OrderNumber, SessionId, Quantity, BookingDate, OriginalPrice, Discount, FinalPrice, CustomerId)
            VALUES 
            (@OrderNumber, @SessionId, @Quantity, @BookingDate, @OriginalPrice, @Discount, @FinalPrice, @CustomerId);
        ";

        _connection.Execute(sql, new
        {
            booking.OrderNumber,
            booking.SessionId,
            booking.Quantity,
            booking.BookingDate,
            booking.OriginalPrice,
            booking.Discount,
            booking.FinalPrice,
            CustomerId = booking.Customer?.Id
        });

        Console.WriteLine($"[DB] Added ticket for {booking.Customer?.Username ?? "Guest"} ({booking.OrderNumber})");
    }

    public List<GroupReservationModel> GetAllBookings() => _bookings;
}
