public class AttractieLogic
{
    private readonly AttractiesAccess _repo;

    public AttractieLogic(AttractiesAccess repo) => _repo = repo;

    public IEnumerable<AttractieModel> GetAll() => _repo.GetAll();
    public AttractieModel? Get(int id) => _repo.GetById(id);

    public void Add(AttractieModel m)
    {
        Validate(m);
        _repo.Insert(m);
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
