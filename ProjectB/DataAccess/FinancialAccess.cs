using Microsoft.Data.Sqlite;
using Dapper;
using System;
using System.Collections.Generic;

public class FinancialAccess
{
    private readonly DatabaseContext _db;

    public FinancialAccess(DatabaseContext db)
    {
        _db = db;
    }
    public  List<ReservationModel> GetAll()
    {
        try
        {
            _db.Connection.Open();
            string sql = "SELECT * FROM Bookings ORDER BY BookingDate DESC;";
            List<ReservationModel> result = _db.Connection.Query<ReservationModel>(sql).AsList();
            return result;
        }
        finally
        {
            _db.Dispose();
        }
    }

    public  List<ReservationModel> GetByDateRange(long from, long to)
    {
        try
        {
            _db.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE BookingDate BETWEEN @From AND @To ORDER BY BookingDate DESC;";
            List<ReservationModel> result = _db.Connection.Query<ReservationModel>(sql, new { From = from, To = to }).AsList();
            return result;
        }
        finally
        {
            _db.Dispose();
        }
    }

    public  List<ReservationModel> GetByCustomer(long customerId)
    {
        try
        {
            _db.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE CustomerId = @CustomerId ORDER BY BookingDate DESC;";
            List<ReservationModel> result = _db.Connection.Query<ReservationModel>(sql, new { CustomerId = customerId }).AsList();
            return result;
        }
        finally
        {
            _db.Dispose();
        }
    }

    public  List<ReservationModel> GetBySession(int sessionId)
    {
        try
        {
            _db.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE SessionId = @SessionId ORDER BY BookingDate DESC;";
            List<ReservationModel> result = _db.Connection.Query<ReservationModel>(sql, new { SessionId = sessionId }).AsList();
            return result;
        }
        finally
        {
            _db.Dispose();
        }
    }
}
