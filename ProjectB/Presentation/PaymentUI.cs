public class PaymentUI
{
    private readonly IPaymentService _paymentService;

    public PaymentUI(IPaymentService paymentService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }

    public async Task<PaymentResultDto> ProcessPaymentAsync(BookingSummaryDto summary, User customer)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Payment ===");
            Console.WriteLine($"Amount: {summary.TotalPrice:C}");
            Console.WriteLine($"Customer: {customer.Name}");
            Console.WriteLine("\n1. Credit/Debit Card\n2. iDEAL\n3. PayPal\nQ. Cancel");

            var method = PromptForPaymentMethod();
            if (method == null)
                return new PaymentResultDto { Success = false, Message = "Payment cancelled" };

            Console.Write("\nProcessing...");
            for (int i = 0; i < 3; i++) { Thread.Sleep(500); Console.Write("."); }

            var result = await _paymentService.ProcessPaymentAsync(summary.OrderNumber, customer, summary.TotalPrice, method.Value);
            
            if (result.Success) return result;
            
            Console.WriteLine($"\nFailed: {result.Message}");
            if (!PromptForChoice("Retry?", "Yes", "No")) return result;
        }
    }

    private PaymentMethods? PromptForPaymentMethod()
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

    private bool PromptForChoice(string message, string yes, string no)
    {
        Console.Write($"\n{message} (y/n): ");
        return Console.ReadLine()?.Trim().ToLower() == "y";
    }
}