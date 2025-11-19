using Microsoft.Data.Sqlite;
using Dapper;
using System.Data;

public static class BirthdayTicketAccess
{
    public const string Table = "BirthdayTickets";

    public static bool HasUsedBirthdayTicket(int userId, int year)
    {
        try
        {
            if (DBC.Connection.State != ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "SELECT COUNT(1) FROM BirthdayTickets WHERE UserId = @UserId AND Year = @Year";
            int count = DBC.Connection.ExecuteScalar<int>(sql, new { UserId = userId, Year = year });
            return count > 0;
        }
        finally
        {
            if (DBC.Connection.State == ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void RecordBirthdayTicketUsage(int userId, int year, string orderNumber)
    {
        try
        {
            if (DBC.Connection.State != ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = "INSERT INTO BirthdayTickets (UserId, Year, OrderNumber, UsedDate) VALUES (@UserId, @Year, @OrderNumber, datetime('now'))";
            DBC.Connection.Execute(sql, new { UserId = userId, Year = year, OrderNumber = orderNumber });
        }
        finally
        {
            if (DBC.Connection.State == ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
}

