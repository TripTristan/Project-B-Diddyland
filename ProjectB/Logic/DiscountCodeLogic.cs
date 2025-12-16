public class DiscountCodeLogic
{
    private readonly DiscountCodeAccess _access;

    public DiscountCodeLogic(DiscountCodeAccess access)
    {
        _access = access;
    }

    public void Create(string code, double percentage)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new Exception("Code cannot be empty.");

        if (percentage <= 0 || percentage > 90)
            throw new Exception("Percentage must be between 1 and 90.");

        _access.AddDiscountCode(code, percentage);
    }

    public double Apply(string? code, double originalPrice)
    {
        if (string.IsNullOrWhiteSpace(code))
            return originalPrice;

        DiscountCodeModel found = _access.GetCode(code);

        if (found == null)
        {
            Console.WriteLine("Invalid or inactive discount code.");
            return originalPrice;
        }

        return originalPrice - (originalPrice/100 * found.Percentage);
    }
}
