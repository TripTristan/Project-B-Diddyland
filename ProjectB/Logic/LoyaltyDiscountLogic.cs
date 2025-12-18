using System;

public class LoyaltyDiscountLogic
{
    private readonly ReservationAccess _reservationAccess;
    private readonly UserAccess _userAccess;

    public LoyaltyDiscountLogic(
        ReservationAccess reservationAccess,
        UserAccess userAccess)
    {
        _reservationAccess = reservationAccess;
        _userAccess = userAccess;
    }

    public bool CanUseLoyaltyDiscount(UserModel user)
    {
        if (user == null)
            return false;

        if (user.Id <= 0)
            return false;

        if (_userAccess.HasUsedLoyaltyDiscount(user.Id))
            return false;

        int distinctVisitDates =
            _reservationAccess.CountDistinctVisitDates(user.Id);

        return distinctVisitDates >= 5;
    }

    public double ApplyAndConsume(UserModel user, double originalPrice)
    {
        if (!CanUseLoyaltyDiscount(user))
            return originalPrice;

        _userAccess.MarkLoyaltyDiscountUsed(user.Id);

        return originalPrice * 0.5;
    }

    public int GetVisitCount(UserModel user)
    {
        if (user == null || user.Id <= 0)
            return 0;

        return _reservationAccess.CountDistinctVisitDates(user.Id);
    }
}
