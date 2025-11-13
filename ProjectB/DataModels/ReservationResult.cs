namespace ProjectB.DataModels
{
    public class ReservationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? OrderNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
