using Microsoft.Data.Sqlite;
using Dapper;

public static class DBC
{
    public static readonly SqliteConnection Connection = new("Data Source=DataSources/diddyland.db");
}