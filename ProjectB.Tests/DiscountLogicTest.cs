using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class DiscountLogicTests
{
    private DiscountLogic _logic = null!;

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Create_EmptyCode_ThrowsException()
    {
        DiscountCodeAccess access = null;
        _logic = new DiscountLogic(access);
        
        _logic.Create("", 50);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Create_ZeroPercentage_ThrowsException()
    {
        DiscountCodeAccess access = null;
        _logic = new DiscountLogic(access);
        
        _logic.Create("TEST", 0);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Create_PercentageAbove90_ThrowsException()
    {
        DiscountCodeAccess access = null;
        _logic = new DiscountLogic(access);
        
        _logic.Create("TEST", 95);
    }

    [TestMethod]
    public void Apply_NullCode_ReturnsOriginalPrice()
    {
        DiscountCodeAccess access = null;
        _logic = new DiscountLogic(access);
        
        double result = _logic.Apply(null, 100.0);

        Assert.AreEqual(100.0, result);
    }

    [TestMethod]
    public void Apply_EmptyCode_ReturnsOriginalPrice()
    {
        DiscountCodeAccess access = null;
        _logic = new DiscountLogic(access);
        
        double result = _logic.Apply("", 100.0);

        Assert.AreEqual(100.0, result);
    }
}