using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

[TestClass]
public class AttractieLogicTests
{
    private Mock<IAttractiesAccess> _mockAccess = null!;
    private AttractieLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAccess = new Mock<IAttractiesAccess>();
        _logic = new AttractieLogic(_mockAccess.Object);
    }

    private AttractieModel ValidModel =>
        new AttractieModel(1, "Rollercoaster", "Thrill Ride", 120, 20, "Zone A");

    [TestMethod]
    public void GetAll_ReturnsItems()
    {
        // Arrange
        var list = new List<AttractieModel> { ValidModel };
        _mockAccess.Setup(a => a.GetAll(null)).Returns(list);

        // Act
        var result = _logic.GetAll();

        // Assert
        Assert.AreEqual(1, new List<AttractieModel>(result).Count);
    }

    [TestMethod]
    public void GetAll_WithLocation_CallsAccessWithLocation()
    {
        // Arrange
        string location = "Zone A";

        // Act
        _logic.GetAll(location);

        // Assert
        _mockAccess.Verify(a => a.GetAll(location), Times.Once);
    }

    [TestMethod]
    public void Get_ById_ReturnsModel()
    {
        // Arrange
        _mockAccess.Setup(a => a.GetById(1)).Returns(ValidModel);

        // Act
        var result = _logic.Get(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Rollercoaster", result!.Name);
    }

    [TestMethod]
    public void Add_ValidModel_CallsInsert()
    {
        // Arrange
        var model = ValidModel;

        // Act
        _logic.Add(model);

        // Assert
        _mockAccess.Verify(a => a.Insert(model), Times.Once);
    }

    [DataTestMethod]
    [DataRow(null, "Thrill Ride", 120, 20, "Zone A")]
    [DataRow("Coaster", null, 120, 20, "Zone A")]
    [DataRow("Coaster", "Ride", -1, 20, "Zone A")]
    [DataRow("Coaster", "Ride", 400, 20, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 0, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 150, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 20, "")]
    public void Add_InvalidModel_ThrowsArgumentException(
        string name, string type, int height, int capacity, string location)
    {
        // Arrange
        var model = new AttractieModel(
            0,
            name ?? "",
            type ?? "",
            height,
            capacity,
            location ?? ""
        );

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _logic.Add(model));
    }

    [TestMethod]
    public void Update_ValidModel_CallsUpdate()
    {
        // Arrange
        var model = ValidModel;

        // Act
        _logic.Update(model);

        // Assert
        _mockAccess.Verify(a => a.Update(model), Times.Once);
    }

    [TestMethod]
    public void Update_MissingId_Throws()
    {
        // Arrange
        var model = new AttractieModel(0, "Coaster", "Ride", 120, 20, "Zone A");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _logic.Update(model));
    }

    [TestMethod]
    public void Update_InvalidModel_Throws()
    {
        // Arrange
        var model = new AttractieModel(1, "", "Ride", 120, 20, "Zone A");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _logic.Update(model));
    }

    [TestMethod]
    public void Delete_ValidId_CallsDelete()
    {
        // Arrange
        int id = 1;

        // Act
        _logic.Delete(id);

        // Assert
        _mockAccess.Verify(a => a.Delete(id), Times.Once);
    }

    [TestMethod]
    public void Delete_InvalidId_Throws()
    {
        // Arrange
        int invalidId = 0;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _logic.Delete(invalidId));
    }
}
