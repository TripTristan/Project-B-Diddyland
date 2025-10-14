public class ReservationUI
{
    private readonly ReservasionLogic _logic;

    public BookingPage(ReservasionLogic logic)
    {
        _logic = logic;
    }


    public void StartReservation()
    {
        Console.WriteLine("===  Reservation ===");

        // string UserName = _logic.GetCurrentUserInfo();









        Booking newBooking = new Booking(customerId);
        _logic.CreateBooking(newBooking);
    }

    private void ShowBookingDetails(Booking booking)
    {
        Console.WriteLine($"Booking ID: {booking.BookingId}");
        Console.WriteLine($"Customer ID: {booking.CustomerId}");
        Console.WriteLine($"Booking Date: {booking.BookingDate}");
    }
}
