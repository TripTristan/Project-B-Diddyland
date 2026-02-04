using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

[TestClass]
public class FinancialLogicTests
{
    private FinancialLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        ReservationAccess reservationAccess = null;
        UserAccess userAccess = null;

        _logic = new FinancialLogic(reservationAccess, userAccess);
    }

    [TestMethod]
    public void GetDateFromCoordinate_ValidCoordinate_ReturnsCorrectDate()
    {
        int[] coord = { 2, 3 }; 
        int year = 2024;
        int month = 3;
        
        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(13, result.Day);
        Assert.AreEqual(3, result.Month);
        Assert.AreEqual(2024, result.Year);
    }

    [TestMethod]
    public void GetDateFromCoordinate_DayExceedsMonth_ReturnsLastDayOfMonth()
    {
        int[] coord = { 10, 0 }; 
        int year = 2024;
        int month = 2; 

        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(29, result.Day);
        Assert.AreEqual(2, result.Month);
    }

    [TestMethod]
    public void GetDateFromCoordinate_ZeroCoordinate_ReturnsFirstDayIfLogicAllows()
    {
        int[] coord = { 0, 1 };
        int year = 2024;
        int month = 1;

        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(1, result.Day);
    }

    [TestMethod]
    public void GetDateFromCoordinate_CheckCalculatedDay()
    {
        int[] coord = { 4, 2 }; 
        int year = 2024;
        int month = 5;

        DateTime result = _logic.GetDateFromCoordinate(coord, year, month);

        Assert.AreEqual(22, result.Day);
    }

    [TestMethod]
    public void GetDateFromCoordinate_CoordNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _logic.GetDateFromCoordinate(null, 2024, 1));
    }

    [TestMethod]
    public void GetDateFromCoordinate_CoordTooShort_ThrowsArgumentException()
    {
        int[] coord = { 1 }; 

        Assert.ThrowsException<ArgumentException>(() =>
            _logic.GetDateFromCoordinate(coord, 2024, 1));
    }

    [TestMethod]
    public void GetDateFromCoordinate_DayZero_ShouldClampToFirstDay()
    {
        int[] coord = { 0, 0 }; 
        DateTime result = _logic.GetDateFromCoordinate(coord, 2024, 1);

        Assert.AreEqual(1, result.Day);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(2024, result.Year);
    }
}