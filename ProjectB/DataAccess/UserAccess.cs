using Microsoft.Data.Sqlite;
using Dapper;

public static class UserAccess
{

    public const string Table = "Account";

    public static void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} (ID, Email, Password, Username, Phone, HeightInCM, DateOfBirth, Admin) VALUES (@Id, @Email, @Password, @Name, @Phone, @Height, @DateOfBirth, 0);";
        DBC.Connection.Execute(sql, account);
        DBC.CloseConnection();
    }

    public static UserModel? GetById(int id)
    {
        string sql = $"SELECT Id, Username AS Name, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin " +
                        $"FROM {Table} WHERE Id = @Id;";
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
    }

    public static UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public static UserModel? GetByUsername(string username)
    {
        const string sql = @"SELECT * FROM Account WHERE Username = @Username;";
        return DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
    }
    public static string? GetNameById(int id)
    {
    const string sql = @"SELECT Username FROM Account WHERE Id = @Id;";
    return DBC.Connection.QueryFirstOrDefault<string>(sql, new { Id = id });
    }

    public static void Update(UserModel account)
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
            DBC.Connection.Execute(sql, account);
            DBC.CloseConnection();
        DBC.Connection.Execute(sql, account);
        DBC.CloseConnection();

    }

    public static void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        DBC.Connection.Execute(sql, new { account.Id });
        DBC.CloseConnection();

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

}