using System;
using Microsoft.Data.Sqlite;

public class DatabaseContext : IDisposable
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteConnection Connection
    {
        get
        {
            if (_connection == null)
                _connection = new SqliteConnection(_connectionString);

            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();

            return _connection;
        }
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
        }
    }
}
