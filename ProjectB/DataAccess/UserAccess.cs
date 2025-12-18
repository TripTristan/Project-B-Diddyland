using System;
using System.Collections.Generic;
using Dapper;

public class UserAccess : IUserAccess
{
    private readonly DatabaseContext _db;
    public const string Table = "Account";

    public UserAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} " +
                     "(ID, Email, Password, Username, Phone, HeightInCM, DateOfBirth, Admin) " +
                     "VALUES (@Id, @Email, @Password, @Name, @Phone, @Height, @DateOfBirth, 0);";

        _db.Connection.Execute(sql, account);
    }

    public UserModel? GetById(int id)
    {
        string sql = $"SELECT Id, Username AS Name, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin " +
                     $"FROM {Table} WHERE Id = @Id;";
        return _db.Connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
    }

    public UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE Email = @Email";
        return _db.Connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public UserModel? GetByUsername(string username)
    {
        const string sql = @"SELECT * FROM Account WHERE Username = @Username;";
        return _db.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
    }

    public string? GetNameById(int id)
    {
        const string sql = @"SELECT Username FROM Account WHERE Id = @Id;";
        return _db.Connection.QueryFirstOrDefault<string>(sql, new { Id = id });
    }

    public IEnumerable<UserModel> GetAllUsers()
    {
        string sql = $"SELECT Id, Username AS Name, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin FROM {Table};";
        return _db.Connection.Query<UserModel>(sql);
    }

    public void SetRole(UserModel user)
    {
        string sql = $"UPDATE {Table} SET Admin = {user.Admin} WHERE ID = {user.Id};";
        _db.Connection.Execute(sql, user);
    }

    public void DeleteUser(int id)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id;";
        _db.Connection.Execute(sql, new { Id = id });
    }

    public void Update(UserModel account)
    {
        string sql = $@"
            UPDATE {Table}
            SET Email = @Email,
                Password = @Password,
                Username = @Name,
                Phone = @Phone,
                HeightInCM = @Height,
                DateOfBirth = @DateOfBirth,
                Admin = @Admin
            WHERE Id = @Id;";

        _db.Connection.Execute(sql, account);
    }

    public void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE Id = @Id";
        _db.Connection.Execute(sql, new { account.Id });
    }

    public int NextId()
    {
        try
        {
            string sql = $"SELECT IFNULL(MAX(Id), 0) + 1 FROM {Table}";
            return _db.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
    }

    public List<string> GetAllUsernames()
    {
        string sql = $"SELECT Username FROM {Table};";
        return _db.Connection.Query<string>(sql).AsList();
    }
}
