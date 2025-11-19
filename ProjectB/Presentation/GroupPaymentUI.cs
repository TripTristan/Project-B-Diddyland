public static class GroupPaymentUI
{
    public static async Task ProcessPaymentAsync(IPaymentService paymentService, GroupReservationDto reservation)
    {
        if (paymentService == null) throw new ArgumentNullException(nameof(paymentService));

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Group Payment ===");
            Console.WriteLine($"Amount: {reservation.FinalPrice:C}");
            Console.WriteLine("\n1. Bank Transfer\n2. Credit Card\n3. iDEAL\nQ. Cancel");

            var method = PromptForPaymentMethod();
            if (method == null)
            {
                Console.WriteLine("\nPayment cancelled.");
                return;
            }

            Console.Write("\nProcessing...");
            for (int i = 0; i < 3; i++) { Thread.Sleep(500); Console.Write("."); }

            var result = await paymentService.ProcessPaymentAsync(reservation.OrderNumber,
                new User(reservation.ContactPerson, reservation.ContactPerson, reservation.ContactPhone,
                    reservation.ContactEmail, "", "", "", ""),
                reservation.FinalPrice, method.Value);

            if (result.Success)
            {
                Console.WriteLine($"\n\nPayment successful! Reference: {result.ConfirmationNumber}");
                return;
            }

            Console.WriteLine($"\nFailed: {result.Message}");
            if (!PromptForChoice("Retry?", "Yes", "No")) return;
        }
    }

    private static PaymentMethods? PromptForPaymentMethod()
    {
        var input = Console.ReadLine()?.Trim().ToUpper();
        return input switch
        {
            "1" => PaymentMethods.CreditDebitCard,
            "2" => PaymentMethods.IDEAL,
            "3" => PaymentMethods.PayPal,
            "Q" => null,
            _ => PromptForPaymentMethod()
        };
    }

    private static bool PromptForChoice(string message, string yes, string no)
    {
        Console.Write($"\n{message} (y/n): ");
        return Console.ReadLine()?.Trim().ToLower() == "y";
    }
}