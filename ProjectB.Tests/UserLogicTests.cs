using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class UserLogicTests
{
    private UserLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        UserAccess access = null;
        _logic = new UserLogic(access);
    }

    [DataTestMethod]
    [DataRow("+31612345678", true)] 
    [DataRow("0612345678", true)]   
    [DataRow("123456", false)]
    [DataRow("+1234567", false)]
    [DataRow("", false)]
    [DataRow("A061234567", false)]
    public void IsPhoneValid_Tests(string phone, bool expected)
    {
        var result = _logic.IsPhoneValid(phone);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(150, true)]
    [DataRow(29, false)]
    [DataRow(251, false)]
    public void IsHeightValid_Tests(int height, bool expected)
    {
        Assert.AreEqual(expected, _logic.IsHeightValid(height));
    }

    [DataTestMethod]
    [DataRow("test@example.com", true)]
    [DataRow("no-at-symbol.com", false)]
    [DataRow("no-dot@com", false)]
    [DataRow("@missingstart.com", false)]
    [DataRow("missingend@", false)]
    public void IsEmailValid_Tests(string email, bool expected)
    {
        Assert.AreEqual(expected, _logic.IsEmailValid(email));
    }

    [DataTestMethod]
    [DataRow("John", true)]
    [DataRow("A", false)]
    [DataRow("ThisNameIsWayTooLongForValidation", false)]
    [DataRow("J0hn", false)]
    public void IsNameValid_Tests(string name, bool expected)
    {
        Assert.AreEqual(expected, _logic.IsNameValid(name));
    }

    [DataTestMethod]
    [DataRow("Aa1!test", true)]
    [DataRow("short1!", false)] 
    [DataRow("NOLOWERCASE1!", false)]
    [DataRow("NoDigits!", false)]
    [DataRow("NoSpecial1", false)]
    public void IsPasswordValid_Tests(string password, bool expected)
    {
        Assert.AreEqual(expected, _logic.IsPasswordValid(password));
    }

    [DataTestMethod]
    [DataRow("01-01-2000", true)]
    [DataRow("2000-01-01", false)]
    [DataRow("01/01/2000", false)]
    [DataRow("notadate", false)]
    public void IsDateOfBirthValid_Tests(string dob, bool expected)
    {
        Assert.AreEqual(expected, _logic.IsDateOfBirthValid(dob));
    }

    [TestMethod]
    public void DOBtoAGE_ComputesCorrectAge()
    {
        var dob = DateTime.Now.AddYears(-20).ToString("dd-MM-yyyy");
        var age = _logic.DOBtoAGE(dob);
        Assert.IsTrue(age >= 19 && age <= 21);
    }
}