// public class BookingPage
// {
//     private readonly BookingLogic _logic;

//     public BookingPage(BookingLogic logic)
//     {
//         _logic = logic;
//     }

//     public void StartBooking(string customerId = "Guest")
//     {
//         Console.WriteLine("=== Start Booking ===");

//         Booking newBooking = new Booking(customerId);
//         _logic.CreateBooking(newBooking);
//     }

//     private void ShowBookingDetails(Booking booking)
//     {
//         Console.WriteLine($"Booking ID: {booking.BookingId}");
//         Console.WriteLine($"Customer ID: {booking.CustomerId}");
//         Console.WriteLine($"Booking Date: {booking.BookingDate}");
//     }
// }
