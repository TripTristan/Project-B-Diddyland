using Microsoft.Data.Sqlite;
using Dapper;

static class UserAccess
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

    public static int HighestId()
    {
        try
        {
            string sql = $"SELECT MAX(Id) FROM {Table}";
            return DBC.Connection.QuerySingleOrDefault<int>(sql);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
}
