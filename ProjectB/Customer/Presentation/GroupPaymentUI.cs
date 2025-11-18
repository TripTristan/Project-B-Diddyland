using System;
using System.Threading;
using Internal;
using ProjectB.DataModels;

namespace ProjectB.Customer.Presentation.GroupReservationServices
{
    public class GroupPaymentUI
    {
        private readonly GroupReservationService _reservationService;

        private readonly GroupPaymentService _paymentService;
        private const int DaysBeforeReminder = 14; // Remind 14 days before the session

        public GroupPaymentUI(GroupReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public void StartPayment(GroupReservationDetails reservation)
        {
            if (reservation == null)
            {
                Console.WriteLine("Error: Invalid reservation details.");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== GROUP BOOKING PAYMENT ===\n");
                
                // Display reservation details
                DisplayReservationDetails(reservation);
                
                // Show payment methods
                Console.WriteLine("\n=== PAYMENT METHODS ===");
                Console.WriteLine("1. Bank Transfer");
                Console.WriteLine("2. Credit Card");
                Console.WriteLine("3. iDEAL");
                Console.WriteLine("Q. Cancel Payment");
                Console.Write("\nSelect payment method (1-3, or Q to cancel): ");

                var input = Console.ReadLine()?.Trim().ToUpper();

                if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\nPayment cancelled. Your group reservation is not confirmed.");
                    Thread.Sleep(1500);
                    return;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 3)
                {
                    ProcessPayment(reservation, choice);
                    break;
                }

                Console.WriteLine("\nInvalid selection. Please try again.");
                Thread.Sleep(1000);
            }
        }

        private void DisplayReservationDetails(GroupReservationDetails reservation)
        {
            Console.WriteLine("=== RESERVATION DETAILS ===");
            Console.WriteLine($"Organization: {reservation.OrganizationName}");
            Console.WriteLine($"Contact Person: {reservation.ContactPerson}");
            Console.WriteLine($"Group Size: {reservation.GroupSize} people");
            Console.WriteLine($"Session Date: {reservation.SessionDate:dddd, MMMM dd, yyyy}");
            Console.WriteLine($"OrganPrijs: {reservation.OrganPrice}");
            Console.WriteLine($"Discount: {reservation.Discount}");
            Console.WriteLine($"Total Amount: €{reservation.FinalPrice:N2}");
            

            var reminderDate = reservation.SessionDate.AddDays(-DaysBeforeReminder);
            var today = DateTime.Today;
            
            Console.WriteLine("\n=== IMPORTANT REMINDERS ===");
            if (today < reminderDate)
            {
                var daysUntilReminder = (reminderDate - today).Days;
                Console.WriteLine($"• Please submit all participant names and ages by {reminderDate:dd MMMM yyyy} ({daysUntilReminder} days from now)");
            }
            else
            {
                Console.WriteLine($"• URGENT: Please submit all participant names and ages as soon as possible!");
            }

            // Show additional requirements based on group type
            Console.WriteLine("\n=== REQUIRED INFORMATION ===");
            switch (reservation.GroupType)
            {
                case GroupType.School:
                    Console.WriteLine("• School authorization letter (signed by principal)");
                    Console.WriteLine("• List of all students and supervising teachers");
                    Console.WriteLine("• Emergency contact numbers for all groups");
                    break;
                case GroupType.Company:
                    Console.WriteLine("• Company registration number");
                    Console.WriteLine("• Purchase order number (if applicable)");
                    Console.WriteLine("• List of all participants");
                    break;
                default:
                    Console.WriteLine("• List of all participants");
                    break;
            }

            Console.WriteLine("\nNote: Failure to submit required information may result in cancellation of your booking.");
        }

        private void ProcessPayment(GroupReservationDetails reservation, int paymentMethod)
        {
            string methodName = paymentMethod switch
            {
                1 => "Bank Transfer",
                2 => "Credit Card",
                3 => "iDEAL",
                _ => "Unknown"
            };

            Console.WriteLine($"\nProcessing payment of €{reservation.TotalAmount:N2} via {methodName}...");
            
            // Simulate payment processing
            Console.Write("Processing");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine();

            // bool paymentSuccessful = new Random().Next(100) < 95;
            (bool paymentSuccessful, string paymentReference) = _paymentService.ProcessPayment(reservation, paymentMethod);

            if (paymentSuccessful)
            {
                
                Console.WriteLine("\n=== PAYMENT SUCCESSFUL ===");
                Console.WriteLine($"Thank you for your payment! Your group booking is now confirmed.");
                Console.WriteLine($"Payment Reference: {paymentReference}");
                
                Console.WriteLine("\n=== NEXT STEPS ===");
                Console.WriteLine("1. You will receive a confirmation email with your booking details");
                Console.WriteLine("2. Please submit all required participant information by the deadline");
                Console.WriteLine("3. Contact us if you need to make any changes to your booking,\n but let op only changes 14 days before the session are possible.\n");
            }
            else
            {
                Console.WriteLine("We are so sorry for the inconvenience,\n please try again or contact with xxxxx@gmail.com or phoine nr 1234567.\n");
            }

            Console.WriteLine("\n\nPress any key to return to the main menu...");
            Console.ReadKey();
        }

        private string GeneratePaymentReference()
        {
            return "GRP" + DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(100, 999);
        }
    }
}
