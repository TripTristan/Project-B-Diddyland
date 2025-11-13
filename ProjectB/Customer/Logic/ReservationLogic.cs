using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectB.DataModels;
using ProjectB.Customer.DataAccess;
using ProjectB.Services;

public class ReservationLogic
{
    private readonly SessionAccess _sessionRepo;
    private readonly ReservationsAccess _bookingRepo;
    private readonly DiscountService _discountService;
    private readonly OfferAccess _offerAccess;

    public ReservationLogic(
        SessionAccess sessionRepo, 
        ReservationsAccess bookingRepo,
        DiscountService discountService,
        OfferAccess offerAccess)
    {
        _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
        _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        _offerAccess = offerAccess ?? throw new ArgumentNullException(nameof(offerAccess));
    }

    public List<Session> GetAvailableSessions()
    {
        var all = _sessionRepo.GetAllSessions();
        return all.Where(s => s.CurrentBookings < s.MaxCapacity).ToList();
    }

    public bool CanBookSession(int sessionId, int quantity)
    {
        var session = _sessionRepo.GetSessionById(sessionId);
        if (session == null) return false;
        return session.CurrentBookings + quantity <= session.MaxCapacity;
    }

    public async Task<ReservationResult> CreateBooking(
        int sessionId, 
        List<TicketOrder> tickets, 
        UserModel? customer, 
        string? promoCode = null)
    {
        var session = _sessionRepo.GetSessionById(sessionId)
            ?? throw new ArgumentException("Invalid session ID.");

        if (session.CurrentBookings + tickets.Count > session.MaxCapacity)
            throw new InvalidOperationException("Not enough available seats.");

        string orderNumber = GenerateOrderNumber(customer);
        
        decimal subtotal = tickets.Sum(t => t.Price);
        decimal discount = await _discountService.CalculateTotalDiscount(
            tickets, 
            promoCode, 
            customer, 
            orderNumber);
        
        decimal finalPrice = Math.Max(0, subtotal - discount);

        var booking = new ReservationModel
        {
            OrderNumber = orderNumber,
            SessionId = sessionId,
            Quantity = tickets.Count,
            BookingDate = DateTime.Now,
            Customer = customer,
            OriginalPrice = subtotal,
            Discount = discount,
            FinalPrice = finalPrice,
            Tickets = tickets.Select(t => new TicketModel
            {
                Age = t.Age,
                Price = t.Price,
                Discount = t.Discount
            }).ToList()
        };

        _bookingRepo.AddBooking(booking);

        session.CurrentBookings += tickets.Count;
        _sessionRepo.UpdateSession(session);

        if (discount > 0 && !string.IsNullOrEmpty(promoCode))
        {
            var offer = await _offerAccess.FindPromoCodeOffer(promoCode);
            if (offer != null)
            {
                await _offerAccess.RecordOfferUsageAsync(new OfferUsageModel
                {
                    OfferId = offer.Id,
                    OrderNumber = orderNumber,
                    CustomerId = customer?.Id,
                    UsedAt = DateTime.Now
                });
            }
        }

        Console.WriteLine($"Booking created. Order number: {orderNumber}, Total: {finalPrice:C} (Discount: {discount:C})");
        
        return new ReservationResult
        {
            Success = true,
            OrderNumber = orderNumber,
            Subtotal = subtotal,
            Discount = discount,
            Total = finalPrice,
            Message = "Booking created successfully"
        };
    }

    public async Task<decimal> CalculateTotalPrice(
        List<TicketOrder> tickets, 
        UserModel? customer, 
        string? promoCode = null)
    {
        decimal subtotal = tickets.Sum(t => t.Price);
        string tempOrderNumber = "TEMP_" + Guid.NewGuid().ToString("N").Substring(0, 10);
        
        decimal discount = await _discountService.CalculateTotalDiscount(
            tickets, 
            promoCode, 
            customer, 
            tempOrderNumber);
            
        return Math.Max(0, subtotal - discount);
    }
    
        
    

    public string GenerateOrderNumber(UserModel? customerInfo)
    {
        var random = new Random();
        int randomNumber = random.Next(1000, 9999);
        string suffix = $"{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";

        if (customerInfo != null)
            return $"ORD-{customerInfo.Id}-{customerInfo.Username}-{suffix}";

        return $"ORD-GUEST-{suffix}";
    }
}
