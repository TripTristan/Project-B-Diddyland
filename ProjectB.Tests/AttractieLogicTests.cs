using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

[TestClass]
public class AttractionLogicTests
{
    private AttractionLogic _logic = null!;

    [TestInitialize]
    public void Setup()
    {
        AttractiesAccess access = null;
        _logic = new AttractionLogic(access);
    }

    private AttractieModel ValidModel =>
        new AttractieModel(1, "Rollercoaster", "Thrill Ride", 120, 20, "Zone A");

    [DataTestMethod]
    [DataRow(null, "Thrill Ride", 120, 20, "Zone A")]
    [DataRow("Coaster", null, 120, 20, "Zone A")]
    [DataRow("Coaster", "Ride", -1, 20, "Zone A")]
    [DataRow("Coaster", "Ride", 400, 20, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 0, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 150, "Zone A")]
    [DataRow("Coaster", "Ride", 120, 20, "")]
    [DataRow("Coaster", "Ride", 0, 20, "Zone A")]      
    [DataRow("Coaster", "Ride", 120, 100, "Zone A")] 
    public void Add_InvalidModel_ThrowsArgumentException(
        string name, string type, int height, int capacity, string location)
    {
        var model = new AttractieModel(
            0,
            name ?? "",
            type ?? "",
            height,
            capacity,
            location ?? ""
        );

        Assert.ThrowsException<ArgumentException>(() => _logic.Add(model));
    }

    [TestMethod]
    public void Update_MissingId_Throws()
    {
        var model = new AttractieModel(0, "Coaster", "Ride", 120, 20, "Zone A");

        Assert.ThrowsException<ArgumentException>(() => _logic.Update(model));
    }

    [TestMethod]
    public void Update_InvalidModel_Throws()
    {
        var model = new AttractieModel(1, "", "Ride", 120, 20, "Zone A");

        Assert.ThrowsException<ArgumentException>(() => _logic.Update(model));
    }

    [TestMethod]
    public void Delete_InvalidId_Throws()
    {
        int invalidId = 0;

        Assert.ThrowsException<ArgumentException>(() => _logic.Delete(invalidId));
    }
}