using Microsoft.Data.Sqlite;
using Dapper;

public static class DBC
{
    public static readonly SqliteConnection Connection = new("Data Source=DataSources/diddyland.db");


    public static void CloseConnection()
{
    
    try
    {
        Connection.Open();

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        Connection.Close();
    }
}
}

