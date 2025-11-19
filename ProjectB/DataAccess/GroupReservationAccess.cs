using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace MyProject.DAL
{
    public static class GroupReservationRepository
    {
        public static List<SessionAvailabilityDto> GetAvailableSessions(int groupSize)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = @"
                    SELECT s.Id, s.Date, s.Time, s.BasisPrice,
                        (s.MaxCapacity - s.CurrentBookings) AS AvailableSeats
                    FROM   [Session] s
                    WHERE  s.MaxCapacity - s.CurrentBookings >= @groupSize";
                return DBC.Connection.Query<SessionAvailabilityDto>(sql, new { groupSize }).ToList();
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static int InsertGroupOrder(GroupReservationDto dto)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = @"
                    INSERT INTO [OrderGroup] (OrderNumber, GroupType, OrganizationName, ContactPerson, ContactEmail, ContactPhone, GroupSize, SessionId, Discount, FinalPrice, CreatedAt)
                    VALUES (@OrderNumber, @GroupType, @OrganizationName, @ContactPerson, @ContactEmail, @ContactPhone, @GroupSize, @SessionId, @Discount, @FinalPrice, datetime('now'));
                    SELECT last_insert_rowid();";
                return DBC.Connection.QuerySingle<int>(sql, dto);
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static int IncrementSessionBooking(int sessionId, int quantity)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = "UPDATE [Session] SET CurrentBookings = CurrentBookings + @quantity WHERE Id = @sessionId";
                return DBC.Connection.Execute(sql, new { sessionId, quantity });
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static string GenerateOrderNumber(string prefix)
            => $"{prefix}-GRP-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
    }
}