public class ReservationLogic
{
    private readonly ReservationAccess _repository;

    public BookingLogic(ReservationAccess repository)
    {
        _repository = repository;
    }

    public void CreateBooking(ReservationModel booking)
    {
        if (booking.CustomerId == "Guest") // not logged in // not aanmeld //""
        {
            _repository.CreateBooking(booking); // 1 by 1

        }

    }
    
    public string GetCurrentUserInfo()
    {
        if (LoginStatus.CurrentUserInfo != null)
        {
            return LoginStatus.CurrentUser.Username;
        }
        else
        {
            return "Guest"; 
        }
        
    }
}

