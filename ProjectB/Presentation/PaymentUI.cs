using MyProject.BLL;

namespace MyProject.UI
{
    public static class PaymentUI
    {
        public static PaymentResultDto ProcessPayment(decimal totalPrice, string orderNumber)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Payment ===");
                Console.WriteLine($"Amount: {totalPrice:C}");
                Console.WriteLine("\n1. Credit/Debit Card\n2. iDEAL\n3. PayPal\nQ. Cancel");

                var method = PromptForPaymentMethod();
                if (method == null)
                    return new PaymentResultDto
                    {
                        Success = false,
                        Message = "Payment cancelled"
                    };

                Console.Write("\nProcessing...");
                for (int i = 0; i < 3; i++) { Thread.Sleep(500); Console.Write("."); }

                // 同步调用
                var result = PaymentService.ProcessPayment(orderNumber, totalPrice, method.Value);

                if (result.Success) return result;

                Console.WriteLine($"\nFailed: {result.Message}");
                if (!PromptForChoice("Retry?", "Yes", "No")) return result;
            }
        }

        private static PaymentMethods? PromptForPaymentMethod()
        {
            var input = Console.ReadLine()?.Trim().ToLower();
            return input switch
            {
                "1" => PaymentMethods.CreditDebitCard,
                "2" => PaymentMethods.IDEAL,
                "3" => PaymentMethods.PayPal,
                "q" => null,
                _ => PromptForPaymentMethod()
            };
        }

        private static bool PromptForChoice(string message, string yes, string no)
        {
            Console.Write($"\n{message} (y/n): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }
    }
}