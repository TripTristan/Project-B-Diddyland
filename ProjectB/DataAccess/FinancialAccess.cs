using Microsoft.Data.Sqlite;
using Dapper;
using System;
using System.Collections.Generic;

public static class FinancialAccess
{
    public static List<ReservationModel> GetAll()
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Bookings ORDER BY BookingDate DESC;";
            List<ReservationModel> result = DBC.Connection.Query<ReservationModel>(sql).AsList();
            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static List<ReservationModel> GetByDateRange(long from, long to)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE BookingDate BETWEEN @From AND @To ORDER BY BookingDate DESC;";
            List<ReservationModel> result = DBC.Connection.Query<ReservationModel>(sql, new { From = from, To = to }).AsList();
            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static List<ReservationModel> GetByCustomer(long customerId)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE CustomerId = @CustomerId ORDER BY BookingDate DESC;";
            List<ReservationModel> result = DBC.Connection.Query<ReservationModel>(sql, new { CustomerId = customerId }).AsList();
            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }

    public static List<ReservationModel> GetBySession(int sessionId)
    {
        try
        {
            DBC.Connection.Open();
            string sql = "SELECT * FROM Bookings WHERE SessionId = @SessionId ORDER BY BookingDate DESC;";
            List<ReservationModel> result = DBC.Connection.Query<ReservationModel>(sql, new { SessionId = sessionId }).AsList();
            return result;
        }
        finally
        {
            DBC.CloseConnection();
        }
    }
}
