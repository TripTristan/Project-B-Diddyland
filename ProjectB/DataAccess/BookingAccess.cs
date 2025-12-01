using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

public static class BookingAccess
{
    public static IEnumerable<BookingModel> GetByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Enumerable.Empty<BookingModel>();

        const string sql = @"
            SELECT
                OrderNumber,
                SessionId         AS SessionId,
                Quantity          AS Quantity,
                BookingDate       AS BookingDate,
                OriginalPrice     AS OriginalPrice,
                Discount          AS Discount,
                FinalPrice        AS FinalPrice,
                CAST(CustomerId AS INTEGER) AS CustomerId,  -- 🔒 force Int64 and alias to exact prop name
                Type              AS Type
            FROM Bookings
            WHERE LOWER(OrderNumber) LIKE LOWER(@pattern);
        ";

        var conn = DBC.Connection;
        var needClose = conn.State != ConnectionState.Open;

        try
        {
            if (needClose) conn.Open();

            var rows = conn.Query<BookingModel>(sql, new { pattern = $"%-{username}-%" }).ToList();

            if (rows.Count == 0)
            {
                rows = conn.Query<BookingModel>(sql, new { pattern = $"%{username}%" }).ToList();
            }

            return rows;
        }
        finally
        {
            if (needClose) conn.Close();
        }
    }
}
