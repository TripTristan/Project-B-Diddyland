using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class LoyaltyDiscountLogicTests
{
    private LoyaltyDiscountLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        ReservationAccess reservationAccess = null;
        UserAccess userAccess = null;

        _logic = new LoyaltyDiscountLogic(reservationAccess, userAccess);
    }

    [TestMethod]
    public void CanUseLoyaltyDiscount_NullUser_ReturnsFalse()
    {
        bool result = _logic.CanUseLoyaltyDiscount(null!);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CanUseLoyaltyDiscount_InvalidId_ReturnsFalse()
    {
        var user = new UserModel { Id = 0 };
        bool result = _logic.CanUseLoyaltyDiscount(user);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetVisitCount_NullUser_ReturnsZero()
    {
        int result = _logic.GetVisitCount(null!);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetVisitCount_InvalidId_ReturnsZero()
    {
        var user = new UserModel { Id = -1 };
        int result = _logic.GetVisitCount(user);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ApplyAndConsume_InvalidUser_ReturnsOriginalPrice()
    {
        double originalPrice = 100.0;
        var user = new UserModel { Id = 0 };

        double result = _logic.ApplyAndConsume(user, originalPrice);

        Assert.AreEqual(originalPrice, result);
    }
}