public static class AttractieLogic
{
    public static IEnumerable<AttractieModel> GetAll(string? location = null) => AttractiesAccess.GetAll(location);

    public static AttractieModel? Get(int id) => AttractiesAccess.GetById(id);

    public static void Add(AttractieModel m)
    {
        Validate(m);
        AttractiesAccess.Insert(m);
    }

    public static void Update(AttractieModel m)
    {
        if (m.ID <= 0) throw new ArgumentException("Missing ID for update.");
        Validate(m);
        AttractiesAccess.Update(m);
    }

    public static void Delete(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid id.");
        AttractiesAccess.Delete(id);
    }

    private static void Validate(AttractieModel m)
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
