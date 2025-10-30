public class AttractieLogic
{
    private readonly AttractiesAccess _repo;

    public AttractieLogic(AttractiesAccess repo) => _repo = repo;

    public IEnumerable<AttractieModel> GetAll() => _repo.GetAll();
    public AttractieModel? Get(int id) => _repo.GetById(id);

    public void Add(AttractieModel m)
{
    try
    {
        Validate(m);
        _repo.Insert(m);
    }
    catch (Exception ex)
    {
        // Log the exception to help with debugging
        Console.WriteLine($"Error occurred while adding the attraction: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        throw new ApplicationException("An error occurred while adding the attraction.", ex);
    }
}



    public void Update(AttractieModel m)
    {
        if (m.ID <= 0) throw new ArgumentException("Missing ID for update.");
        Validate(m);
        _repo.Update(m);
    }

    public void Delete(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid id.");
        _repo.Delete(id);
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
    }
}