using Microsoft.Data.Sqlite;
using Dapper;


public static class ReservationAccess
{
    private static readonly List<ReservationModel> _bookings = new();
    public const string Table = "Bookings";

    public static void AddBooking(ReservationModel booking)
    {
        string sql =
            $"INSERT INTO {Table} (OrderNumber, SessionID, Quantity, CustomerId, BookingDate, OriginalPrice, Discount, FinalPrice) " +
            "VALUES (@OrderNumber, @SessionId, @Quantity, @CustomerID, @BookingDate, @OriginalPrice, @Discount, @FinalPrice);";

        DBC.Connection.Execute(sql, booking);
        Console.WriteLine($"[DB] Added ticket for {UserAccess.GetNameById(booking.CustomerID) ?? "Guest"} ({booking.OrderNumber})");
        DBC.CloseConnection();
    }

    public static List<ReservationModel> GetAllBookings() => _bookings;
}
