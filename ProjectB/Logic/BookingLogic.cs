public class BookingLogic
{
    private readonly BookingRepository _repository;

    public BookingLogic(BookingRepository repository)
    {
        _repository = repository;
    }

    public void CreateBooking(Booking booking)
    {
        if (booking.CustomerId == "Guest") // not logged in // not aanmeld //""
        {
            _repository.CreateBooking(booking); // 1 by 1

        }
        
    }
}