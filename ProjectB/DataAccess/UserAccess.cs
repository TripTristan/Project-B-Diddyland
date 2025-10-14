using Microsoft.Data.Sqlite;
using Dapper;

public class UsersAccess
{
    private readonly SqliteConnection _connection = new($"Data Source=DataSources/diddyland.db");

    public void Insert(UserModel user)
    {
        const string sql = @"INSERT INTO Account (Admin, FirstName, LastName, Email, Password, HeightInCM, DateOfBirth)
                             VALUES (@Admin, @FirstName, @LastName, @Email, @Password, @HeightInCM, @DateOfBirth)";
        _connection.Execute(sql, user);
    }

    public UserModel? GetByEmail(string email)
    {
        const string sql = "SELECT * FROM Account WHERE Email = @Email";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public IEnumerable<UserModel> GetAll()
    {
        const string sql = "SELECT * FROM Account";
        return _connection.Query<UserModel>(sql);
    }

    public void Update(UserModel user)
    {
        const string sql = @"UPDATE Account SET 
                             FirstName = @FirstName,
                             LastName = @LastName,
                             Email = @Email,
                             Password = @Password,
                             HeightInCM = @HeightInCM,
                             DateOfBirth = @DateOfBirth
                             WHERE AccountID = @AccountID";
        _connection.Execute(sql, user);
    }

    public void Delete(int accountId)
    {
        const string sql = "DELETE FROM Account WHERE AccountID = @AccountID";
        _connection.Execute(sql, new { AccountID = accountId });
    }
}
