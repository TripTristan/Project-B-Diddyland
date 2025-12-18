using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class FinancialLogicTests
{
    private Mock<ReservationAccess> _mockReservationAccess = null!;
    private Mock<UserAccess> _mockUserAccess = null!;
    private FinancialLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockReservationAccess = new Mock<ReservationAccess>();
        _mockUserAccess = new Mock<UserAccess>();
        _logic = new FinancialLogic(
            _mockReservationAccess.Object,
            _mockUserAccess.Object
        );
    }

    [TestMethod]
    public void GetDateFromCoordinate_ValidCoordinate_ReturnsCorrectDate()
    {
        int[] coord = { 1, 2 };
        int year = 2024;
        int month = 3;

        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(new DateTime(2024, 3, 10), result);
    }

    [TestMethod]
    public void GetDateFromCoordinate_DayExceedsMonth_ReturnsLastDayOfMonth()
    {
        int[] coord = { 5, 6 };
        int year = 2024;
        int month = 2;

        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(new DateTime(2024, 2, 29), result);
    }

    [TestMethod]
    public void GetRevenueByDateRange_ReturnsOrders()
    {
        long beginDate = 20240101;
        long endDate = 20240131;

        var reservations = new List<ReservationModel>
        {
            new ReservationModel(),
            new ReservationModel()
        };

        _mockReservationAccess
            .Setup(r => r.GetAllOrdersBetweenDates(beginDate, endDate))
            .Returns(reservations);

        var result = _logic.GetRevenueByDateRange(beginDate, endDate);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetRevenueByDateRange_CallsAccessOnce()
    {
        long beginDate = 20240101;
        long endDate = 20240131;

        _logic.GetRevenueByDateRange(beginDate, endDate);

        _mockReservationAccess.Verify(
            r => r.GetAllOrdersBetweenDates(beginDate, endDate),
            Times.Once
        );
    }

    [TestMethod]
    public void GetAllUserOrders_ReturnsOrdersForUser()
    {
        var user = new UserModel { Id = 1 };

        var orders = new List<ReservationModel>
        {
            new ReservationModel()
        };

        _mockReservationAccess
            .Setup(r => r.GetAllBookingsByUserID(user.Id))
            .Returns(orders);

        var result = _logic.GetAllUserOrders(user);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetAllUserOrders_CallsAccessWithCorrectUserId()
    {
        var user = new UserModel { Id = 5 };

        _logic.GetAllUserOrders(user);

        _mockReservationAccess.Verify(
            r => r.GetAllBookingsByUserID(user.Id),
            Times.Once
        );
    }

    [TestMethod]
    public void GrabAllUsers_ReturnsUsers()
    {
        var users = new List<UserModel>
        {
            new UserModel(),
            new UserModel()
        };

        _mockUserAccess
            .Setup(u => u.GetAllUsers())
            .Returns(users);

        var result = _logic.GrabAllUsers();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GrabAllUsers_CallsAccessOnce()
    {
        _logic.GrabAllUsers();

        _mockUserAccess.Verify(
            u => u.GetAllUsers(),
            Times.Once
        );
    }
}
