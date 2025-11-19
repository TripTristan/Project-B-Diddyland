using System;
using System.Collections.Generic;
using System.Linq;
using MyProject.DAL;

namespace MyProject.BLL
{
    public static class GroupReservationLogic
    {
        public const int MIN_GROUP_SIZE = 20;

        public static GroupReservationDto CreateReservation(
            GroupType type,
            string orgName,
            string contactPerson,
            string email,
            string phone,
            int groupSize,
            int sessionId,
            string userId = "GUEST")
        {

            if (groupSize < MIN_GROUP_SIZE)
                throw new ArgumentException($"Minimum {MIN_GROUP_SIZE} people required.");

            decimal discount = CalculateGroupDiscount(groupSize);
            var session = GroupReservationRepository.GetAvailableSessions(groupSize)
                            .FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
                throw new ArgumentException("Session not available for this group size.");

            decimal finalPrice = groupSize * session.BasisPrice * (1 - discount);

            var dto = new GroupReservationDto
            {
                OrderNumber = GroupReservationRepository.GenerateOrderNumber(userId),
                GroupType = type,
                OrganizationName = orgName,
                ContactPerson = contactPerson,
                ContactEmail = email,
                ContactPhone = phone,
                GroupSize = groupSize,
                SessionId = sessionId,
                Discount = discount,
                FinalPrice = finalPrice
            };

            dto.Id = GroupReservationRepository.InsertGroupOrder(dto);
            GroupReservationRepository.IncrementSessionBooking(sessionId, groupSize);

            return dto;
        }

        public static (bool isValid, GroupType type) ValidateGroupTypeChoice(string? input)
        {
            if (!int.TryParse(input, out int choice)) return (false, GroupType.None);
            return choice switch
            {
                0 => (true, GroupType.None),
                1 => (true, GroupType.School),
                2 => (true, GroupType.Company),
                _ => (false, GroupType.None)
            };
        }

        private static decimal CalculateGroupDiscount(int groupSize)
        {
            if (groupSize >= 50) return 0.25m;
            if (groupSize >= 30) return 0.20m;
            if (groupSize >= 20) return 0.15m;
            return 0m;
        }
    }
}