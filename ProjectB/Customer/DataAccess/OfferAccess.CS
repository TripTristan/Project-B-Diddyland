using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

public class OfferAccess
{
    private readonly SqliteConnection _conn =
        new($"Data Source=DataSources/project.db");

    public int Insert(OfferModel offer)
    {
        const string sql = @"
            INSERT INTO Offer(Name,Description,Discount,StartDate,EndDate,IsActive,TargetOnlyCustomers)
            VALUES(@Name,@Description,@Discount,@StartDate,@EndDate,@IsActive,@TargetOnlyCustomers);
            SELECT last_insert_rowid();";
        var id = _conn.ExecuteScalar<int>(sql, offer);

        foreach (var r in offer.Rules)
        {
            r.OfferId = id;
            const string rs = @"INSERT INTO OfferRule(OfferId,RuleType,RuleValue)
                                VALUES(@OfferId,@RuleType,@RuleValue)";
            _conn.Execute(rs, r);
        }
        return id;
    }

    public void Update(OfferModel offer)
    {
        const string sql = @"UPDATE Offer
            SET Name=@Name,Description=@Description,Discount=@Discount,
            StartDate=@StartDate,EndDate=@EndDate,IsActive=@IsActive,
            TargetOnlyCustomers=@TargetOnlyCustomers
            WHERE Id=@Id";
        _conn.Execute(sql, offer);

        _conn.Execute("DELETE FROM OfferRule WHERE OfferId=@Id", new { offer.Id });
        foreach (var r in offer.Rules)
        {
            r.OfferId = offer.Id;
            _conn.Execute(
                "INSERT INTO OfferRule(OfferId,RuleType,RuleValue)VALUES(@OfferId,@RuleType,@RuleValue)", r);
        }
    }

    public void SetActive(int id, bool active) =>
        _conn.Execute("UPDATE Offer SET IsActive=@active WHERE Id=@id", new { id, active });

    public IEnumerable<OfferModel> GetAll()
    {
        const string sql = "SELECT * FROM Offer";
        var list = _conn.Query<OfferModel>(sql).ToList();
        foreach (var o in list)
            o.Rules = GetRules(o.Id).ToList();
        return list;
    }

    public OfferModel? GetById(int id)
    {
        const string sql = "SELECT * FROM Offer WHERE Id=@id";
        var o = _conn.QuerySingleOrDefault<OfferModel>(sql, new { id });
        if (o != null) o.Rules = GetRules(id).ToList();
        return o;
    }

    private IEnumerable<OfferRuleModel> GetRules(int offerId) =>
        _conn.Query<OfferRuleModel>(
            "SELECT * FROM OfferRule WHERE OfferId=@offerId", new { offerId });

    public bool UsageExists(int offerId, string orderNumber) =>
        _conn.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM OfferUsage WHERE OfferId=@offerId AND OrderNumber=@orderNumber",
            new { offerId, orderNumber }) > 0;

    public void InsertUsage(OfferUsageModel u) =>
        _conn.Execute(
            @"INSERT INTO OfferUsage(OfferId,OrderNumber,CustomerId,UsedAt)
              VALUES(@OfferId,@OrderNumber,@CustomerId,@UsedAt)", u);
}