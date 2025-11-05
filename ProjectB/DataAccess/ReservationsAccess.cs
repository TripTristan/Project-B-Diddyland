using Microsoft.Data.Sqlite;
using Dapper;


public static class ReservationAccess
{
    private static readonly List<ReservationModel> _bookings = new();
    public const string Table = "Bookings";

    public static void AddBooking(ReservationModel booking)
    {
        

        // Console.WriteLine(booking.OrderNumber);
        // Console.WriteLine(booking.SessionId);
        // Console.WriteLine(booking.Quantity);
        // Console.WriteLine(booking.CustomerID);
        // Console.WriteLine(booking.BookingDate);
        // Console.WriteLine(booking.OriginalPrice);
        // Console.WriteLine(booking.Discount);
        // Console.WriteLine(booking.FinalPrice);
        string sql = $"INSERT INTO {Table} (OrderNumber, SessionID, Quantity, BookingDate, OriginalPrice, Discount, FinalPrice, CustomerId) VALUES (@OrderNumber, @SessionId, @Quantity, @CustomerID, @BookingDate, @OriginalPrice, @Discount, @FinalPrice);";
        DBC.Connection.Execute(sql, booking);
        Console.WriteLine($"[DB] Added ticket for {UserAccess.GetNameById(booking.CustomerID) ?? "Guest"} ({booking.OrderNumber})");
        DBC.CloseConnection();
    }

    public static List<ReservationModel> GetAllBookings() => _bookings;
}
