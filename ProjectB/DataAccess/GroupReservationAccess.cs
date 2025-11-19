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
            const string sql = @"
                SELECT s.Id, s.Date, s.Time, s.BasisPrice,
                    (s.MaxCapacity - s.CurrentBookings) AS AvailableSeats
                FROM   [Session] s
                WHERE  s.MaxCapacity - s.CurrentBookings >= @groupSize";
            using var conn = DBC.Connection;
            return conn.Query<SessionAvailabilityDto>(sql, new { groupSize }).ToList();
        }

        public static int InsertGroupOrder(GroupReservationDto dto)
        {
            const string sql = @"
                INSERT INTO [OrderGroup] (OrderNumber, GroupType, OrganizationName, ContactPerson, ContactEmail, ContactPhone, GroupSize, SessionId, Discount, FinalPrice, CreatedAt)
                VALUES (@OrderNumber, @GroupType, @OrganizationName, @ContactPerson, @ContactEmail, @ContactPhone, @GroupSize, @SessionId, @Discount, @FinalPrice, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";
                using var conn = DBC.Connection;
            return conn.QuerySingle<int>(sql, dto);
        }

        public static int IncrementSessionBooking(int sessionId, int quantity)
        {
            const string sql = "UPDATE [Session] SET CurrentBookings = CurrentBookings + @quantity WHERE Id = @sessionId";
            using var conn = DBC.Connection;
            return conn.Execute(sql, new { sessionId, quantity });
        }

        public static string GenerateOrderNumber(string prefix)
            => $"{prefix}-GRP-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
    }
}