using Microsoft.Data.Sqlite;
using Dapper;

public static class DBC
{
    public static readonly SqliteConnection Connection = new("Data Source=C:/Users/ahmad/Desktop/School/Project B/Project-B-Diddyland/ProjectB/DataSources/diddyland.db");


    public static void CloseConnection()
{
    
    try
    {
        Connection.Open();
        Console.WriteLine("Database connection opened.");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        Connection.Close();
        Console.WriteLine("Database connection closed.");
    }
}
}

