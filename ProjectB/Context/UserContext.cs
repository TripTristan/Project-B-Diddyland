public class UserContext
{
    public LoginStatus loginStatus { get; }
    public UserAuthentication userAuth { get; }

    public ReservationLogic reservationLogic { get; }
    public FastPassLogic fastPassLogic { get; }
    public BookingHistoryLogic bookingHistoryLogic { get; }
    public UserLogic userLogic { get; }
    public FoodmenuLogic foodmenuLogic { get; }
    public ComplaintLogic complaintLogic { get; }
    public AuthenticationLogic authenticationLogic { get; }
    public DiscountLogic discountLogic { get; }


    public UserContext(
        LoginStatus loginStatus,
        UserAuthentication userAuth,
        ReservationLogic reservationLogic,
        FastPassLogic fastPassLogic,
        BookingHistoryLogic bookingHistoryLogic,
        UserLogic userLogic,
        FoodmenuLogic foodmenuLogic,
        ComplaintLogic complaintLogic,
        AuthenticationLogic authenticationLogic,
        DiscountLogic discountLogic)
    {
        this.loginStatus = loginStatus;
        this.authenticationLogic = authenticationLogic;
        this.userAuth = userAuth;
        this.reservationLogic = reservationLogic;
        this.fastPassLogic = fastPassLogic;
        this.bookingHistoryLogic = bookingHistoryLogic;
        this.userLogic = userLogic;
        this.foodmenuLogic = foodmenuLogic;
        this.complaintLogic = complaintLogic;
        this.discountLogic = discountLogic;
    }
}
