using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

[TestClass]
public class UserLogicTests
{
    private Mock<IUserAccess> _mockAccess = null!;
    private UserLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAccess = new Mock<IUserAccess>();
        _logic = new UserLogic(_mockAccess.Object);
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
        // Act
        var result = _logic.IsPhoneValid(phone);

        // Assert
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
    [DataRow("short1!", false)]   // no uppercase
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
        // Arrange – pretend the user was born exactly 20 years ago
        var dob = DateTime.Now.AddYears(-20).ToString("dd-MM-yyyy");

        // Act
        var age = _logic.DOBtoAGE(dob);

        // Assert
        Assert.IsTrue(age >= 19 && age <= 21); // allow ±1 day due to leap years
    }

    [TestMethod]
    public void Register_CallsWriteWithCorrectUser()
    {
        // Arrange
        _mockAccess.Setup(a => a.NextId()).Returns(10);

        UserModel? captured = null;

        _mockAccess
            .Setup(a => a.Write(It.IsAny<UserModel>()))
            .Callback<UserModel>(u => captured = u);

        // Act
        _logic.Register("John", "john@example.com", "01-01-2000", 180, "0612345678", "Aa1!test");

        // Assert
        Assert.IsNotNull(captured);
        Assert.AreEqual(10, captured!.Id);
        Assert.AreEqual("John", captured.Name);
        Assert.AreEqual("john@example.com", captured.Email);
        Assert.AreEqual("01-01-2000", captured.DateOfBirth);
        Assert.AreEqual(180, captured.Height);
        Assert.AreEqual("0612345678", captured.Phone);
        Assert.AreEqual("Aa1!test", captured.Password);

        _mockAccess.Verify(a => a.Write(It.IsAny<UserModel>()), Times.Once);
    }
}
