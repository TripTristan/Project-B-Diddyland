using System.Collections.Generic;
using System.Linq;
using Dapper;

public class DiscountCodeAccess
{
    private readonly DatabaseContext _db;

    public DiscountCodeAccess(DatabaseContext db)
    {
        _db = db;
    }

    public void AddDiscountCode(string code, int percent)
    {
        string sql = @"INSERT INTO DiscountCodes (Code, Percentage, Active)
                       VALUES (@Code, @Percentage, 1);";
        _db.Connection.Execute(sql, new { Code = code, Percentage = percent });
    }

    public DiscountCodeModel? GetCode(string code)
    {
        string sql = @"SELECT * FROM DiscountCodes 
                       WHERE LOWER(Code) = LOWER(@Code) AND Active = 1;";
        return _db.Connection.Query<DiscountCodeModel>(sql, new { Code = code }).FirstOrDefault();
    }
}
