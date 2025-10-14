using Microsoft.Data.Sqlite;
using Dapper;

public static class DBC
{
    public static readonly SqliteConnection Connection = new("Data Source=C:/Users/ahmad/Desktop/School/Project B/Project-B-Diddyland/ProjectB/Database/diddyland.db");
}