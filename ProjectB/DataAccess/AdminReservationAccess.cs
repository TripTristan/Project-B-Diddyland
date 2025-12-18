using Dapper;
using System.Collections.Generic;
using System.Linq;

public class AdminReservationAccess
{
    private readonly DatabaseContext _db;

    public AdminReservationAccess(DatabaseContext db)
    {
        _db = db;
    }

    public List<BookingModel> GetAllBookings()
        => _db.Connection.Query<BookingModel>(
            @"SELECT 
                OrderNumber,
                SessionId,
                Quantity,
                BookingDate,
                REPLACE(Price, ',', '.') AS Price,
                CustomerId,
                CASE Type
                    WHEN 0 THEN 'Reservation'
                    WHEN 1 THEN 'FastPass'
                END AS Type
            FROM Bookings
            ORDER BY BookingDate DESC"
        ).ToList();

    public List<BookingModel> GetBookingsByUser(int userId)
        => _db.Connection.Query<BookingModel>(
        @"SELECT 
            OrderNumber,
            SessionId,
            Quantity,
            BookingDate,
            REPLACE(Price, ',', '.') AS Price,
            CustomerId,
            CASE Type
                WHEN 0 THEN 'Reservation'
                WHEN 1 THEN 'FastPass'
            END AS Type
        FROM Bookings
        WHERE CustomerId = @Id
        ORDER BY BookingDate DESC",
            new { Id = userId }
        ).ToList();

    public List<BookingModel> GetBookingsBySessionDate(long dateTicks)
        => _db.Connection.Query<BookingModel>(
            @"SELECT 
                b.OrderNumber,
                b.SessionId,
                b.Quantity,
                b.BookingDate,
                REPLACE(b.Price, ',', '.') AS Price,
                b.CustomerId,
                CASE b.Type
                    WHEN 0 THEN 'Reservation'
                    WHEN 1 THEN 'FastPass'
                END AS Type
            FROM Bookings b
            JOIN Sessions s ON s.Id = b.SessionId
            WHERE s.Date = @Date
            ORDER BY s.Time",
            new { Date = dateTicks }
        ).ToList();

    public BookingModel? GetByOrder(string orderNumber)
        => _db.Connection.QueryFirstOrDefault<BookingModel>(
            @"SELECT 
                OrderNumber,
                SessionId,
                Quantity,
                BookingDate,
                REPLACE(Price, ',', '.') AS Price,
                CustomerId,
                Type
              FROM Bookings
              WHERE OrderNumber = @Order",
            new { Order = orderNumber }
        );

    public void Insert(BookingModel b)
        => _db.Connection.Execute(
            @"INSERT INTO Bookings
              (OrderNumber, SessionId, Quantity, CustomerId, BookingDate, Price, Type)
              VALUES
              (@OrderNumber, @SessionId, @Quantity, @CustomerId, @BookingDate, @Price, @Type)",
            b
        );

    public void Update(BookingModel b)
        => _db.Connection.Execute(
            @"UPDATE Bookings
              SET SessionId = @SessionId,
                  Quantity  = @Quantity,
                  Price     = @Price,
                  Type      = @Type
              WHERE OrderNumber = @OrderNumber",
            b
        );
    
   public List<UserModel> GetAllUsers()
    => _db.Connection.Query<UserModel>(
        @"SELECT 
            Id,
            Username AS Name,
            Email,
            Admin AS Role
          FROM Account
          WHERE Admin = 0"
    ).ToList();



    public void Delete(string orderNumber)
        => _db.Connection.Execute(
            @"DELETE FROM Bookings WHERE OrderNumber = @Order",
            new { Order = orderNumber }
        );
}
