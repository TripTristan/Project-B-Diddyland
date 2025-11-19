using Microsoft.Data.Sqlite;
using Dapper;

public static class UserAccess
{

    public const string Table = "Account";

    public static void Write(UserModel account)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"INSERT INTO {Table} (ID, Email, Password, Username, Phone, HeightInCM, DateOfBirth, Admin) VALUES (@Id, @Email, @Password, @Name, @Phone, @Height, @DateOfBirth, 0);";
            DBC.Connection.Execute(sql, account);
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static UserModel? GetById(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"SELECT Id, Username AS Name, Username, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin " +
                            $"FROM {Table} WHERE Id = @Id;";
            var result = DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
            if (result != null && string.IsNullOrEmpty(result.Username))
                result.Username = result.Name;
            return result;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static UserModel? GetByEmail(string email)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"SELECT Id, Username AS Name, Username, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin FROM {Table} WHERE email = @Email";
            var result = DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
            if (result != null && string.IsNullOrEmpty(result.Username))
                result.Username = result.Name;
            return result;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static UserModel? GetByUsername(string username)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"SELECT Id, Username AS Name, Username, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin FROM Account WHERE Username = @Username;";
            var result = DBC.Connection.QueryFirstOrDefault<UserModel>(sql, new { Username = username });
            if (result != null && string.IsNullOrEmpty(result.Username))
                result.Username = result.Name;
            return result;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
    public static string? GetNameById(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            const string sql = @"SELECT Username FROM Account WHERE Id = @Id;";
            return DBC.Connection.QueryFirstOrDefault<string>(sql, new { Id = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static IEnumerable<UserModel> GetAllUsers()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"SELECT Id, Username AS Name, Username, Email, DateOfBirth, HeightInCM AS Height, Phone, Password, Admin FROM {Table};";
            IEnumerable<UserModel> users = DBC.Connection.Query<UserModel>(sql);
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.Username))
                    user.Username = user.Name;
            }
            return users;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void SetRole(int id, int roleLevel)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"UPDATE {Table} SET Admin = @Role WHERE ID = @Id;";
            DBC.Connection.Execute(sql, new { Id = id, Role = roleLevel });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }
    
    public static void DeleteUser(int id)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"DELETE FROM {Table} WHERE ID = @Id;";
            DBC.Connection.Execute(sql, new { Id = id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Update(UserModel account)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

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
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static void Delete(UserModel account)
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"DELETE FROM {Table} WHERE id = @Id";
            DBC.Connection.Execute(sql, new { account.Id });
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

    public static int NextId()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"SELECT IFNULL(MAX(Id), 0) + 1 FROM {Table}";
            return DBC.Connection.ExecuteScalar<int>(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting next ID: " + e.Message);
            return 1;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

        public static List<string> GetAllUsernames()
    {
        try
        {
            if (DBC.Connection.State != System.Data.ConnectionState.Open)
                DBC.Connection.Open();

            string sql = $"SELECT Username FROM {Table};";
            List<string> usernames = DBC.Connection.Query<string>(sql).AsList();
            return usernames;
        }
        finally
        {
            if (DBC.Connection.State == System.Data.ConnectionState.Open)
                DBC.Connection.Close();
        }
    }

}