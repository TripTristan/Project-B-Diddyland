using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectB.DataModels;

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

    public async Task<IEnumerable<OfferModel>> GetAllActiveOffers(DateTime currentDate)
    {
        const string sql = @"
            SELECT * FROM Offer 
            WHERE IsActive = 1 
            AND StartDate <= @currentDate 
            AND EndDate >= @currentDate";
        
        var offers = (await _conn.QueryAsync<OfferModel>(sql, new { currentDate })).ToList();
        
        // Load rules for each offer
        foreach (var offer in offers)
        {
            offer.Rules = (await GetOfferRules(offer.Id)).ToList();
        }
        
        return offers;
    }

    private async Task<IEnumerable<OfferRuleModel>> GetOfferRules(int offerId)
    {
        const string sql = "SELECT * FROM OfferRule WHERE OfferId = @offerId";
        return await _conn.QueryAsync<OfferRuleModel>(sql, new { offerId });
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

    public async Task<bool> IsPromoCodeUsed(string promoCode, int? customerId)
    {
        const string sql = @"
            SELECT COUNT(1) 
            FROM Offer o
            JOIN OfferRule r ON o.Id = r.OfferId
            JOIN OfferUsage u ON o.Id = u.OfferId
            WHERE r.RuleType = 'PromoCode' 
            AND r.RuleValue LIKE @promoCodePattern
            AND (u.CustomerId = @customerId OR u.OrderNumber IS NOT NULL)";
        
        var promoCodePattern = $@"%""Code"":""{promoCode}""%";
        return await _conn.ExecuteScalarAsync<int>(sql, new { promoCodePattern, customerId }) > 0;
    }

    public async Task RecordOfferUsageAsync(OfferUsageModel usage)
    {
        const string sql = @"
            INSERT INTO OfferUsage (OfferId, OrderNumber, CustomerId, UsedAt)
            VALUES (@OfferId, @OrderNumber, @CustomerId, @UsedAt)";
        
        await _conn.ExecuteAsync(sql, usage);
    }

    public async Task<OfferModel> FindPromoCodeOffer(string promoCode)
    {
        const string sql = @"
            SELECT o.* 
            FROM Offer o
            JOIN OfferRule r ON o.Id = r.OfferId
            WHERE r.RuleType = 'PromoCode'
            AND r.RuleValue LIKE @promoCodePattern";
        
        var promoCodePattern = $@"%""Code"":""{promoCode}""%";
        var offer = await _conn.QueryFirstOrDefaultAsync<OfferModel>(sql, new { promoCodePattern });
        
        if (offer != null)
        {
            offer.Rules = (await GetOfferRules(offer.Id)).ToList();
        }
        
        return offer;
    }

    public async Task IncrementPromoCodeUsage(int offerId, string promoCode)
    {
        var offer = await GetById(offerId);
        if (offer == null) return;

        var promoRule = offer.Rules.FirstOrDefault(r => r.RuleType == "PromoCode" && 
            r.RuleValue.Contains($@"""Code"":""{promoCode}"""));
        
        if (promoRule != null)
        {
            var parameters = promoRule.GetRuleValue<RuleParameters.PromoCodeRuleParameters>();
            if (parameters != null)
            {
                parameters.CurrentUses++;
                promoRule.SetRuleValue(parameters);
                
                const string sql = "UPDATE OfferRule SET RuleValue = @RuleValue WHERE Id = @Id";
                await _conn.ExecuteAsync(sql, new { RuleValue = promoRule.RuleValue, Id = promoRule.Id });
            }
        }
    }

    public void InsertUsage(OfferUsageModel u) =>
        _conn.Execute(
            @"INSERT INTO OfferUsage(OfferId,OrderNumber,CustomerId,UsedAt)
              VALUES(@OfferId,@OrderNumber,@CustomerId,@UsedAt)", u);
}