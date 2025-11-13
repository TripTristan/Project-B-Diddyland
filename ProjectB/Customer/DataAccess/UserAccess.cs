using Microsoft.Data.Sqlite;
using Dapper;

public class UserAccess
{
    public const string Table = "Account";

    public static void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} (ID, Email, Password, Username, Phone, HeightInCM, DateOfBirth, Admin) VALUES (@Id, @Email, @Password, @Name, @Phone, @Height, @DateOfBirth, 0);";
        DBC.Connection.Execute(sql, account);
    }

    public static UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public UserModel? GetByUsername(string username)
    {
        const string sql = @"SELECT * FROM Account WHERE Username = @Username;";
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
    }

    public static void Update(UserModel account)
    {
        string sql = $"UPDATE {Table} SET email = @EmailAddress, password = @Password, fullname = @FullName WHERE id = @Id";
        DBC.Connection.Execute(sql, account);
    }

    public static void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        DBC.Connection.Execute(sql, new { account.Id });
    }

    public static int NextId()
    {
        try
        {
            string sql = $"SELECT IFNULL(MAX(Id), 0) + 1 FROM {Table}";
            return DBC.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
    }

    public UserModel? GetAdminByUsername(string username)
    {
        const string sql = @"
            SELECT a.*, 
                   CASE WHEN a.Admin = 1 THEN 'Admin' ELSE 'User' END as Role 
            FROM Account a 
            WHERE a.Username = @Username AND a.Admin = 1;";
            
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
    }
}