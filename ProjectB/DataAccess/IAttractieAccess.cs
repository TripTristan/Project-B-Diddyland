public interface IAttractiesAccess
{
    void Insert(AttractieModel attractie);
    IEnumerable<AttractieModel> GetAll(string? location = null);
    AttractieModel? GetById(int id);
    void Update(AttractieModel attractie);
    void Delete(int id);
}
