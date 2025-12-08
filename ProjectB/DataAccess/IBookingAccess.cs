using System.Collections.Generic;

public interface IBookingAccess
{
    IEnumerable<BookingModel> GetByUsername(string username);
}
