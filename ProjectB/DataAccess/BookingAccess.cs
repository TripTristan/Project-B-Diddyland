using System.Collections.Generic;
using System.Linq;
using Dapper;

public class BookingAccess : IBookingAccess
{
    private readonly DatabaseContext _db;

    public BookingAccess(DatabaseContext db)
    {
        _db = db;
    }

    public IEnumerable<BookingModel> GetByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Enumerable.Empty<BookingModel>();

        const string sql = @"
            SELECT 
                OrderNumber,
                SessionId AS SessionId,
                Quantity AS Quantity,
                BookingDate AS BookingDate,
                CAST(REPLACE(Price, ',', '.') AS REAL) AS Price,
                CAST(CustomerId AS INTEGER) AS CustomerId,
                Type AS Type
            FROM Bookings
            WHERE LOWER(OrderNumber) LIKE LOWER(@pattern);
        ";

        var pattern1 = $"%-{username}-%";

        var rows = _db.Connection
            .Query<BookingModel>(sql, new { pattern = pattern1 })
            .ToList();

        if (rows.Count == 0)
        {
            var pattern2 = $"%{username}%";

            rows = _db.Connection
                .Query<BookingModel>(sql, new { pattern = pattern2 })
                .ToList();
        }

        return rows;
    }
}
