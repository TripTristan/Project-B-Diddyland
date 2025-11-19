using Microsoft.Data.Sqlite;
using Dapper;

public static class DBC
{
    public static readonly SqliteConnection Connection = new("Data Source=DataSources/diddyland.db");


    public static void CloseConnection()
    {
        try
        {
            if (Connection.State != System.Data.ConnectionState.Closed)
            {
                Connection.Close();
                Console.WriteLine("Database connection closed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing connection: {ex.Message}");
        }
    }
}

