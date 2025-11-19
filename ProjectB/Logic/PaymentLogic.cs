using MyProject.DAL;

namespace MyProject.BLL
{
    public static class PaymentLogic
    {

        public static PaymentResultDto ProcessPayment(
            string orderNumber,
            decimal amount,
            PaymentMethods method)
        {
            try
            {
                if (!PaymentRepository.SimulateGateway(method))
                    return new PaymentResultDto
                    {
                        Success = false,
                        Message = "Payment declined"
                    };

                string confirmationNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();

                PaymentRepository.InsertPayment(orderNumber, amount, method.ToString(), confirmationNumber);

                return new PaymentResultDto
                {
                    Success = true,
                    ConfirmationNumber = confirmationNumber,
                    Message = "Payment processed"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Payment error: {ex.Message}");
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "System error"
                };
            }
        }
    }
}