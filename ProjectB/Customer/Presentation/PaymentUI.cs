using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ProjectB.DataModels;

public static class PaymentUI
{
    public static void StartPayment(
        string orderNumber, 
        UserModel customer,
        Dictionary<int, Dictionary<int, List<int>>> bookingSelections,
        List<IGrouping<string, Session>> groupedByDate,
        Dictionary<string, decimal> discountDescriptions,
        decimal totalFinalPrice)
    {
        var paymentMethods = new Dictionary<string, string>
        {
            {"1", "Credit/Debit Card"},
            {"2", "iDEAL"},
            {"3", "PayPal"}
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Payment Confirmation ===");
            Console.WriteLine("Please review your booking details before proceeding with payment.\n");

            DisplayBookingSummary(
                orderNumber,
                bookingSelections,
                groupedByDate,
                discountDescriptions,
                totalFinalPrice
            );
            
            DisplayCustomerInfo(customer);

            Console.WriteLine("\n=== Payment Method ===");
            Console.WriteLine("1. Credit/Debit Card");
            Console.WriteLine("2. iDEAL");
            Console.WriteLine("3. PayPal");
            Console.Write("\nSelect payment method (1-3, or 'Q' to cancel): ");
            
            var input = Console.ReadLine()?.Trim();
            
            if (input?.Equals("Q", StringComparison.OrdinalIgnoreCase) == true)
            {
                Console.WriteLine("\nPayment cancelled by user.");
                Thread.Sleep(1500); // Wait for 1.5 seconds
                return;
            }

            if (paymentMethods.TryGetValue(input, out var selectedMethod))
            {
                Console.WriteLine($"\nYou have selected: {selectedMethod}");
                ProcessPayment(orderNumber, selectedMethod, customer, totalFinalPrice);
                break;
            }

            Console.WriteLine("\n Invalid selection. Please try again.");
            Thread.Sleep(1500);
        }
    }

    private static void DisplayBookingSummary(
        string orderNumber,
        Dictionary<int, Dictionary<int, List<int>>> bookingSelections,
        List<IGrouping<string, Session>> groupedByDate,
        Dictionary<string, decimal> discountDescriptions,
        decimal totalFinalPrice)
    {
        Console.WriteLine("=== Booking Details ===");
        Console.WriteLine($"Order Number: {orderNumber}");
        Console.WriteLine($"Booking Date: {DateTime.Now:dd-MM-yyyy}");
        Console.WriteLine("-----------------------------------------------------------");

        foreach (var (dateChoice, sessionBookings) in bookingSelections)
        {
            var dateGroup = groupedByDate[dateChoice];
            
            if (!DateTime.TryParse(dateGroup.Key, out var sessionDate))
            {
                Console.WriteLine("\n Warning: Could not parse session date. Using current date instead.");
                sessionDate = DateTime.Today;
            }
            
            Console.WriteLine($"\nDate: {sessionDate:dddd, MMMM dd, yyyy}");

            foreach (var (sessionId, ages) in sessionBookings)
            {
                var session = dateGroup.FirstOrDefault(s => s.Id == sessionId);
                if (session == null) continue;

                int childCount = ages.Count(age => age < 12);
                int seniorCount = ages.Count(age => age >= 65);
                int adultCount = ages.Count(age => age >= 12 && age < 65);

                Console.WriteLine($"\n  Session: {session.Time} - {session.Name}");
                if (childCount > 0) Console.WriteLine($"    Child Tickets (0-11): {childCount}");
                if (adultCount > 0) Console.WriteLine($"    Adult Tickets (12-64): {adultCount}");
                if (seniorCount > 0) Console.WriteLine($"    Senior Tickets (65+): {seniorCount}");

                decimal sessionSubtotal = (childCount * 15m) + (adultCount * 25m) + (seniorCount * 20m);
                Console.WriteLine($"    Session Subtotal: {sessionSubtotal:C}");
            }
        }

        if (discountDescriptions?.Any() == true)
        {
            Console.WriteLine("\n-----------------------------------------------------------");
            Console.WriteLine("Applied Discounts:");
            foreach (var (description, amount) in discountDescriptions)
            {
                Console.WriteLine($"  {description}: -{amount:C}");
            }
        }

        Console.WriteLine("\n-----------------------------------------------------------");
        Console.WriteLine($"TOTAL AMOUNT: {totalFinalPrice:C}");
        Console.WriteLine("===========================================================");
    }

    private static void DisplayCustomerInfo(UserModel customer)
    {
        Console.WriteLine("\n=== Customer Information ===");
        Console.WriteLine($"Name: {customer.Name}");
        Console.WriteLine($"Email: {customer.Email}");
        Console.WriteLine($"Phone: {customer.Phone}");
    }

    private static void ProcessPayment(string orderNumber, string paymentMethod, UserModel customer, decimal totalAmount)
    {
        Console.WriteLine($"\nProcessing payment of {totalAmount:C} via {paymentMethod}...");
        
        Console.Write("Processing");
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(500);
            Console.Write(".");
        }
        Console.WriteLine("\n");

        bool paymentSuccessful = SimulatePaymentProcessing();

        if (paymentSuccessful)
        {
            Console.WriteLine("Payment successful!");
            Console.WriteLine("Your tickets have been booked and will be sent to your email.");
            Console.WriteLine($"Confirmation number: {GenerateConfirmationNumber()}");

        }
        else
        {
            Console.WriteLine("Payment failed. Please try again or contact support.");
        }

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static bool SimulatePaymentProcessing()
    {
        return new Random().Next(100) < 95;
    }

    private static string GenerateConfirmationNumber()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}
