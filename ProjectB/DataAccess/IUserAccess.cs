using System.Collections.Generic;

public interface IUserAccess
{
    void Write(UserModel account);
    UserModel? GetById(int id);
    UserModel? GetByEmail(string email);
    UserModel? GetByUsername(string username);
    string? GetNameById(int id);
    IEnumerable<UserModel> GetAllUsers();
    void SetRole(int id, int roleLevel);
    void DeleteUser(int id);
    void Update(UserModel account);
    void Delete(UserModel account);
    int NextId();
    List<string> GetAllUsernames();
}
