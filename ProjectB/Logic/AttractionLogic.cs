using System;
using System.Collections.Generic;

public class AttractionLogic
{
    private readonly IAttractiesAccess _attractiesAccess;

    public AttractionLogic(IAttractiesAccess attractiesAccess)
    {
        _attractiesAccess = attractiesAccess;
    }

    public IEnumerable<AttractieModel> GetAll(string? location = null)
        => _attractiesAccess.GetAll(location);

    public AttractieModel? Get(int id)
        => _attractiesAccess.GetById(id);

    public void Add(AttractieModel m)
    {
        Validate(m);
        _attractiesAccess.Insert(m);
    }

    public void Update(AttractieModel m)
    {
        if (m.ID <= 0) throw new ArgumentException("Missing ID for update.");
        Validate(m);
        _attractiesAccess.Update(m);
    }

    public void Delete(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid id.");
        _attractiesAccess.Delete(id);
    }

    private void Validate(AttractieModel m)
    {
        if (string.IsNullOrWhiteSpace(m.Name))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(m.Type))
            throw new ArgumentException("Type is required.");
        if (m.MinHeightInCM < 0 || m.MinHeightInCM > 300)
            throw new ArgumentException("Min height must be between 0 and 300 cm.");
        if (m.Capacity <= 0 || m.Capacity > 100)
            throw new ArgumentException("Capacity must be between 1 and 100.");
        if (string.IsNullOrWhiteSpace(m.Location))
            throw new ArgumentException("Location is required.");
    }
}
