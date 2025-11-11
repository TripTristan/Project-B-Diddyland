using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

public class OfferAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";

    private SqliteConnection GetConnection() => new SqliteConnection(_connectionString);

    public int Insert(OfferModel offer)
    {
        using var conn = GetConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            const string sql = @"
                INSERT INTO Offer(Name, Description, Discount, StartDate, EndDate, IsActive, TargetOnlyCustomers)
                VALUES(@Name, @Description, @Discount, @StartDate, @EndDate, @IsActive, @TargetOnlyCustomers);
                SELECT last_insert_rowid();";
                
            var id = conn.ExecuteScalar<int>(sql, offer, transaction);

            foreach (var r in offer.Rules)
            {
                r.OfferId = id;
                const string rs = @"
                    INSERT INTO OfferRule(OfferId, RuleType, RuleValue)
                    VALUES(@OfferId, @RuleType, @RuleValue)";
                conn.Execute(rs, r, transaction);
            }

            transaction.Commit();
            return id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Update(OfferModel offer)
    {
        using var conn = GetConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            const string sql = @"
                UPDATE Offer
                SET Name = @Name, 
                    Description = @Description, 
                    Discount = @Discount,
                    StartDate = @StartDate, 
                    EndDate = @EndDate, 
                    IsActive = @IsActive,
                    TargetOnlyCustomers = @TargetOnlyCustomers
                WHERE Id = @Id";
                
            conn.Execute(sql, offer, transaction);

            conn.Execute("DELETE FROM OfferRule WHERE OfferId = @Id", new { offer.Id }, transaction);
            
            foreach (var r in offer.Rules)
            {
                r.OfferId = offer.Id;
                conn.Execute(
                    "INSERT INTO OfferRule(OfferId, RuleType, RuleValue) VALUES(@OfferId, @RuleType, @RuleValue)", 
                    r, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void SetActive(int id, bool active)
    {
        using var conn = GetConnection();
        conn.Execute("UPDATE Offer SET IsActive = @active WHERE Id = @id", new { id, active });
    }

    public void Delete(int id)
    {
        using var conn = GetConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            conn.Execute("DELETE FROM OfferRule WHERE OfferId = @id", new { id }, transaction);
            var affected = conn.Execute("DELETE FROM Offer WHERE Id = @id", new { id }, transaction);
            // conn.Execute("DELETE FROM OfferUsage WHERE OfferId = @id", new { id }, transaction);// keep usage history ??
            
            transaction.Commit();
            
            if (affected == 0)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public IEnumerable<OfferModel> GetAll()
    {
        using var conn = GetConnection();
        const string sql = "SELECT * FROM Offer";
        var list = conn.Query<OfferModel>(sql).ToList();
        
        foreach (var o in list)
            o.Rules = GetRules(conn, o.Id).ToList();
            
        return list;
    }

    public OfferModel GetById(int id)
    {
        using var conn = GetConnection();
        const string sql = "SELECT * FROM Offer WHERE Id = @id";
        var o = conn.QuerySingleOrDefault<OfferModel>(sql, new { id });
        
        if (o != null) 
            o.Rules = GetRules(conn, id).ToList();
            
        return o;
    }

    private IEnumerable<OfferRuleModel> GetRules(SqliteConnection conn, int offerId) =>
        conn.Query<OfferRuleModel>(
            "SELECT * FROM OfferRule WHERE OfferId = @offerId", new { offerId });

    public bool UsageExists(int offerId, string orderNumber)
    {
        using var conn = GetConnection();
        return conn.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM OfferUsage WHERE OfferId = @offerId AND OrderNumber = @orderNumber",
            new { offerId, orderNumber }) > 0;
    }

    public void InsertUsage(OfferUsageModel usage)
    {
        using var conn = GetConnection();
        conn.Execute(
            @"INSERT INTO OfferUsage(OfferId, OrderNumber, CustomerId, UsedAt)
              VALUES(@OfferId, @OrderNumber, @CustomerId, @UsedAt)", usage);
    }
}