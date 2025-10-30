using Microsoft.Data.Sqlite;
using Dapper;

public class UserAccess
{
    DBC DATABASE = new();

    public const string Table = "Account";

    public void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} (ID, Email, Password, Username, Phone, HeightInCM, DateOfBirth, Admin) VALUES (@Id, @Email, @Password, @Name, @Phone, @Height, @DateOfBirth, 0);";
        DATABASE.Connection.Execute(sql, account);
        DATABASE.CloseConnection();
    }

    public UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        return DATABASE.Connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public UserModel? GetByUsername(string username)
    {
        const string sql = @"SELECT * FROM Account WHERE Username = @Username;";
        return DATABASE.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
    }

    public void Update(UserModel account)
    {
        string sql = $"UPDATE {Table} SET email = @EmailAddress, password = @Password, fullname = @FullName WHERE id = @Id";
        DATABASE.Connection.Execute(sql, account);
        DATABASE.CloseConnection();

    }

    public void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        DATABASE.Connection.Execute(sql, new { account.Id });
        DATABASE.CloseConnection();

    }

    public int NextId()
    {
        try
        {
            string sql = $"SELECT IFNULL(MAX(Id), 0) + 1 FROM {Table}";
            return DATABASE.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
    }

}