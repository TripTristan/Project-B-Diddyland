using System;

enum UserRole
{
    User = 0,
    Admin = 1,
    SuperAdmin = 2
}

partial class Program
{
    static void Main()
    {
        DatabaseContext db = new DatabaseContext("Data Source=DataSources/diddyland.db");
        Dependencies DP = new(db);
        DP.app.Run();
    }
}