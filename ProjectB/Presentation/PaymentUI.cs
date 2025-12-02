public class PaymentUI
{
    public PaymentUI() { }

    public void StartPayment(string orderNumber, UserModel? customer)
    {
        Console.WriteLine($"[PAYMENT] Processing order {orderNumber} for {(customer?.Email ?? "guest")}...");
        Console.WriteLine("[PAYMENT] Payment completed (stub).");
    }
}
