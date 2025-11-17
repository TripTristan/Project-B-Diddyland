public class GroupReservationModel : ReservationModel
{
    public GroupReservationModel(
        string orderNumber,
        int sessionId,
        int quantity,
        UserModel? customer,
        DateTime bookingDate,
        decimal originalPrice,
        decimal discount,
        decimal finalPrice) :
        base(orderNumber,
        sessionId,
        quantity,
        customer,
        bookingDate,
        originalPrice,
        discount,
        finalPrice)
    { }
}



