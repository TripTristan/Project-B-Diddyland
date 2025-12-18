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

    private ReservationModel ValidReservation => new ReservationModel();
    private UserModel ValidUser => new UserModel { Id = 1 };

    [TestMethod]
    public void GetDateFromCoordinate_ValidCoordinate_ReturnsCorrectDate()
    {
        // Arrange
        int[] coord = { 1, 2 };
        int year = 2024;
        int month = 3;

        // Act
        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        // Assert
        Assert.AreEqual(new DateTime(2024, 3, 10), result);
    }

    [TestMethod]
    public void GetDateFromCoordinate_DayExceedsMonth_ReturnsLastDayOfMonth()
    {
        // Arrange
        int[] coord = { 5, 6 };
        int year = 2024;
        int month = 2;

        // Act
        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        // Assert
        Assert.AreEqual(new DateTime(2024, 2, 29), result);
    }

    [TestMethod]
    public void GetRevenueByDateRange_ReturnsOrders()
    {
        // Arrange
        long beginDate = 20240101;
        long endDate = 20240131;

        var orders = new List<ReservationModel>
        {
            ValidReservation,
            ValidReservation
        };

        _mockReservationAccess
            .Setup(r => r.GetAllOrdersBetweenDates(beginDate, endDate))
            .Returns(orders);

        // Act
        var result = _logic.GetRevenueByDateRange(beginDate, endDate);

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetRevenueByDateRange_CallsAccessWithCorrectDates()
    {
        // Arrange
        long beginDate = 20240101;
        long endDate = 20240131;

        // Act
        _logic.GetRevenueByDateRange(beginDate, endDate);

        // Assert
        _mockReservationAccess.Verify(
            r => r.GetAllOrdersBetweenDates(beginDate, endDate),
            Times.Once
        );
    }

    [TestMethod]
    public void GetAllUserOrders_ReturnsOrdersForUser()
    {
        // Arrange
        var user = ValidUser;

        var orders = new List<ReservationModel> { ValidReservation };

        _mockReservationAccess
            .Setup(r => r.GetAllBookingsByUserID(user.Id))
            .Returns(orders);

        // Act
        var result = _logic.GetAllUserOrders(user);

        // Assert
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetAllUserOrders_CallsAccessWithCorrectUserId()
    {
        // Arrange
        var user = new UserModel { Id = 5 };

        // Act
        _logic.GetAllUserOrders(user);

        // Assert
        _mockReservationAccess.Verify(
            r => r.GetAllBookingsByUserID(user.Id),
            Times.Once
        );
    }

    [TestMethod]
    public void GrabAllUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new UserModel(),
            new UserModel()
        };

        _mockUserAccess
            .Setup(u => u.GetAllUsers())
            .Returns(users);

        // Act
        var result = _logic.GrabAllUsers();

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GrabAllUsers_CallsAccessOnce()
    {
        // Act
        _logic.GrabAllUsers();

        // Assert
        _mockUserAccess.Verify(u => u.GetAllUsers(), Times.Once);
    }
}
