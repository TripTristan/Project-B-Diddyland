public static class GroupPaymentLogic
{
    public static PaymentResultDto ProcessGroupPayment(
        string orderNumber,
        decimal amount,
        PaymentMethods method)
    {
        try
        {
            if (GroupPaymentRepository.IsOrderPaid(orderNumber))
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Order already paid"
                };

            if (!GroupPaymentRepository.SimulateGateway())
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Payment declined"
                };

            string confirmationNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();
            GroupPaymentRepository.InsertGroupPayment(orderNumber, amount, method.ToString(), confirmationNumber);

            return new PaymentResultDto
            {
                Success = true,
                ConfirmationNumber = confirmationNumber,
                Message = "Group payment processed"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GroupPayment error: {ex.Message}");
            return new PaymentResultDto
            {
                Success = false,
                Message = "System error"
            };
        }
    }
}
