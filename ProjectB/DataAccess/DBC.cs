using Microsoft.Data.Sqlite;
using Dapper;

public class DBC
{
    public readonly SqliteConnection Connection;
    public DBC()
    {
        Connection = new("Data Source=C:/Users/ahmad/Desktop/School/Project B/Project-B-Diddyland/ProjectB/DataSources/diddyland.db");
    }

    public void CloseConnection()
{
    var connection = new SqliteConnection("Data Source=DataSources/diddyland.db;");
    
    try
    {
        connection.Open();
        Console.WriteLine("Database connection opened.");

        // Perform your database operations here

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        // Close the connection when done
        connection.Close();
        Console.WriteLine("Database connection closed.");
    }
}
}

