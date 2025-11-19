using System;
using System.Collections.Generic;
using System.Linq;
using MyProject.BLL;

namespace MyProject.UI
{
    public static class GroupReservationUI
    {

        public static void ShowReservationFlow()
        {
            Console.Clear();
            Console.WriteLine("=== Group Booking System ===\n");

            var type = SelectGroupType();
            if (type == GroupType.None) return;

            var (orgName, contactPerson, email, phone, groupSize) = CollectGroupInfo();

            if (groupSize < GroupReservationLogic.MIN_GROUP_SIZE)
            {
                Console.WriteLine($"\nError: Minimum {GroupReservationLogic.MIN_GROUP_SIZE} people required.");
                Console.ReadKey();
                return;
            }

            var sessions = GroupReservationRepository.GetAvailableSessions(groupSize);
            if (!sessions.Any())
            {
                Console.WriteLine("\nNo available sessions for your group size.");
                Console.ReadKey();
                return;
            }
            var session = SelectSession(sessions);
            if (session == null) return;

            var reservation = GroupReservationLogic.CreateReservation(
                type, orgName, contactPerson, email, phone, groupSize, session.Id,
                userId: LoginStatus.CurrentUser?.Id ?? "GUEST");

            DisplayReservationDetails(reservation);

            if (!Confirm("\nConfirm reservation?", "Yes", "No")) return;

            GroupPaymentUI.ProcessPayment(reservation);
        }

        private static GroupType SelectGroupType()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select group type:");
                Console.WriteLine("1. School Group");
                Console.WriteLine("2. Company Group");
                Console.WriteLine("0. Back");
                Console.Write("\nSelect (0-2): ");

                var (ok, type) = GroupReservationLogic.ValidateGroupTypeChoice(Console.ReadLine());
                if (ok) return type;
                Console.WriteLine("Invalid selection.");
            }
        }

        private static (string orgName, string contactPerson, string email, string phone, int groupSize)
            CollectGroupInfo()
        {
            Console.Clear();
            Console.WriteLine("=== Group Information ===\n");

            string orgName, contactPerson, email, phone;
            int groupSize;

            do { Console.Write("Organization Name: "); }
            while (string.IsNullOrWhiteSpace(orgName = Console.ReadLine()));

            do { Console.Write("Contact Person: "); }
            while (string.IsNullOrWhiteSpace(contactPerson = Console.ReadLine()));

            do { Console.Write("Email: "); }
            while (string.IsNullOrWhiteSpace(email = Console.ReadLine()) || !email.Contains("@"));

            do { Console.Write("Phone: "); }
            while (string.IsNullOrWhiteSpace(phone = Console.ReadLine()));

            do
            {
                Console.Write($"Group size (min {GroupReservationLogic.MIN_GROUP_SIZE}): ");
            } while (!int.TryParse(Console.ReadLine(), out groupSize));

            return (orgName, contactPerson, email, phone, groupSize);
        }

        private static SessionAvailabilityDto? SelectSession(List<SessionAvailabilityDto> sessions)
        {
            Console.WriteLine("\nAvailable Sessions:");
            for (int i = 0; i < sessions.Count; i++)
                Console.WriteLine($"{i + 1}. {sessions[i].Date} - {sessions[i].Time}");

            while (true)
            {
                Console.Write($"\nSelect (1-{sessions.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= sessions.Count)
                    return sessions[choice - 1];
                Console.WriteLine("Invalid selection.");
            }
        }

        private static void DisplayReservationDetails(GroupReservationDto details)
        {
            Console.Clear();
            Console.WriteLine("=== Reservation Details ===\n");
            Console.WriteLine($"Order: {details.OrderNumber}");
            Console.WriteLine($"Organization: {details.OrganizationName}");
            Console.WriteLine($"Contact: {details.ContactPerson}");
            Console.WriteLine($"Group Size: {details.GroupSize}");
            Console.WriteLine($"Discount: {details.Discount:P}");
            Console.WriteLine($"Total: {details.FinalPrice:C}");
        }

        private static bool Confirm(string message, string yes, string no)
        {
            Console.Write($"\n{message} (y/n): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }
    }
}