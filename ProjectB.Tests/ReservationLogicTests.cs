using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

public class TestReservationLogic
{
    public const int MaxNormalReservation = 10;
    public const int MinGroupReservation = 10;
    public const int MaxGroupReservation = 30;
    public const double GroupDiscountRate = 0.20;

    public void ValidateReservationType(int totalGuests, ReservationType type)
    {
        if (type == ReservationType.Normal && totalGuests > MaxNormalReservation)
            throw new InvalidOperationException("Normal reservations allow up to 10 people.");

        if (type == ReservationType.Group &&
            (totalGuests < MinGroupReservation || totalGuests > MaxGroupReservation))
            throw new InvalidOperationException("Group reservations require 10–30 people.");
    }

    public double ApplyGroupDiscount(double basePrice)
    {
        return basePrice * (1 - GroupDiscountRate);
    }

    public double CalculatePriceForGuests(List<int> guests)
    {
        return (guests[0] * 5) + (guests[1] * 15) + (guests[2] * 7.50);
    }

    public string GenerateOrderNumber(UserModel? customerInfo)
    {
        var random = new Random();
        int randomNumber = random.Next(1000, 9999);
        string suffix = $"{DateTime.Now:yyyyMMddHHmmss}-{randomNumber}-{Guid.NewGuid().ToString()[..4]}";

        if (customerInfo != null)
            return $"ORD-{customerInfo.Id}-{customerInfo.Username}-{suffix}";

        return $"ORD-GUEST-{suffix}";
    }

    public string AvailabilityFormatter(SessionModel session)
    {
        List<string> timeslots = new() { "", "09:00-13:00", "13:00-17:00", "17:00-21:00" };

        if (session.Capacity > 10)
            return $"✅ {timeslots[(int)session.Time]} | {session.Capacity - 5} slots available";

        if (session.Capacity <= 5 && session.Capacity > 0)
            return $"💫 {timeslots[(int)session.Time]} | {session.Capacity} VIP slots left";

        if (session.Capacity == 0)
            return $"⭕ {timeslots[(int)session.Time]} | FULL";

        return $"⚠️ {timeslots[(int)session.Time]} | LIMITED";
    }
}

[TestClass]
public class ReservationLogicTests
{
    private TestReservationLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        _logic = new TestReservationLogic();
    }

    [TestMethod]
    public void ApplyGroupDiscount_ReturnsCorrectPrice()
    {
        double basePrice = 100.0;
        double result = _logic.ApplyGroupDiscount(basePrice);
        Assert.AreEqual(80.0, result);
    }

    [TestMethod]
    public void CalculatePriceForGuests_ReturnsCorrectSum()
    {
        var guests = new List<int> { 1, 1, 1 }; 
        double result = _logic.CalculatePriceForGuests(guests);
        Assert.AreEqual(27.50, result);
    }

    [TestMethod]
    public void ValidateReservationType_NormalValid_DoesNotThrow()
    {
        _logic.ValidateReservationType(8, ReservationType.Normal);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValidateReservationType_NormalTooMany_ThrowsException()
    {
        _logic.ValidateReservationType(11, ReservationType.Normal);
    }

    [TestMethod]
    public void AvailabilityFormatter_FullSession_ReturnsFullString()
    {
        var session = new SessionModel(1, DateTime.Now.Ticks, 1, 0);
        string result = _logic.AvailabilityFormatter(session);
        Assert.IsTrue(result.Contains("FULL"));
    }

    [TestMethod]
    public void GenerateOrderNumber_Guest_ContainsGuestString()
    {
        string result = _logic.GenerateOrderNumber(null);
        Assert.IsTrue(result.Contains("ORD-GUEST-"));
    }
}