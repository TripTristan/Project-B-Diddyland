public static class AppFactory
{
    public static UserLoginUI CreateLoginUI()
    {
        var repo = new UserAccess();           
        var logic = new LoginLogic(repo);       
        var ui = new UserLoginUI(logic);
        return ui;
    }

    public static ReservationUI CreateReservationUI()
    {
        var sessionRepo = new SessionAccess();
        var bookingRepo = new ReservationsAccess(); 
        var logic = new ReservationLogic(sessionRepo, bookingRepo);
        var ui = new ReservationUI(logic);
        return ui;
    }
}
