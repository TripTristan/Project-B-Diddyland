using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace MyProject.DAL
{
    public static class OfferManagementRepository
    {
        public static List<OfferBase> GetAllOffers()
        {
            const string sqlBase = "SELECT * FROM OfferBase WHERE IsActive = 1";
            const string sqlVip  = "SELECT * FROM OfferVIP  WHERE IsActive = 1";
            const string sqlGroup= "SELECT * FROM OfferGroup WHERE IsActive = 1";
            const string sqlPromo= "SELECT * FROM OfferPromoCode WHERE (ExpiryDate IS NULL OR ExpiryDate >= GETDATE())";

            using var conn = DBC.Connection;
            var baseList = conn.Query<OfferBase>(sqlBase).ToList();
            var vipList  = conn.Query<OfferVIP>(sqlVip).Cast<OfferBase>().ToList();
            var grpList  = conn.Query<OfferGroup>(sqlGroup).Cast<OfferBase>().ToList();
            var proList  = conn.Query<OfferPromoCode>(sqlPromo).Cast<OfferBase>().ToList();

            return baseList.Union(vipList).Union(grpList).Union(proList).ToList();
        }

        public static OfferBase? GetOfferById(int id)
        {
            using var conn = DBC.Connection;
            if (conn.QueryFirstOrDefault<OfferBase>("SELECT * FROM OfferBase WHERE Id = @id", new { id }) != null)
                return conn.QueryFirstOrDefault<OfferBase>("SELECT * FROM OfferBase WHERE Id = @id", new { id });
            if (conn.QueryFirstOrDefault<OfferVIP>("SELECT * FROM OfferVIP WHERE Id = @id", new { id }) != null)
                return conn.QueryFirstOrDefault<OfferVIP>("SELECT * FROM OfferVIP WHERE Id = @id", new { id });
            if (conn.QueryFirstOrDefault<OfferGroup>("SELECT * FROM OfferGroup WHERE Id = @id", new { id }) != null)
                return conn.QueryFirstOrDefault<OfferGroup>("SELECT * FROM OfferGroup WHERE Id = @id", new { id });
            return conn.QueryFirstOrDefault<OfferPromoCode>("SELECT * FROM OfferPromoCode WHERE Id = @id", new { id });
        }

        /* --------------- 增 --------------- */
        public static int InsertOffer(OfferBase offer)
        {
            const string sqlBase = @"
                INSERT INTO OfferBase (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry)
                VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlVip = @"
                INSERT INTO OfferVIP (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry)
                VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlGroup = @"
                INSERT INTO OfferGroup (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry, GroupType)
                VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry, @GroupType);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            const string sqlPromo = @"
                INSERT INTO OfferPromoCode (Nr, PromoCode, ExpiryDate, MaxUses, CurrentUses, Discount)
                VALUES (@Nr, @PromoCode, @ExpiryDate, @MaxUses, @CurrentUses, @Discount);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = DBC.Connection;
            return offer switch
            {
                OfferVIP     => conn.QuerySingle<int>(sqlVip, offer),
                OfferGroup   => conn.QuerySingle<int>(sqlGroup, offer),
                OfferPromoCode => conn.QuerySingle<int>(sqlPromo, offer),
                _            => conn.QuerySingle<int>(sqlBase, offer)
            };
        }

        /* --------------- 改 --------------- */
        public static int UpdateOffer(OfferBase offer)
        {
            const string sqlBase = @"
                UPDATE OfferBase
                SET Nr=@Nr, Name=@Name, Description=@Description, Discount=@Discount,
                    StartDate=@StartDate, EndDate=@EndDate, [Min]=@Min, [Max]=@Max,
                    IsActive=@IsActive, DaysBeforeExpiry=@DaysBeforeExpiry
                WHERE Id = @Id";

            const string sqlVip = @"
                UPDATE OfferVIP
                SET Nr=@Nr, Name=@Name, Description=@Description, Discount=@Discount,
                    StartDate=@StartDate, EndDate=@EndDate, [Min]=@Min, [Max]=@Max,
                    IsActive=@IsActive, DaysBeforeExpiry=@DaysBeforeExpiry
                WHERE Id = @Id";

            const string sqlGroup = @"
                UPDATE OfferGroup
                SET Nr=@Nr, Name=@Name, Description=@Description, Discount=@Discount,
                    StartDate=@StartDate, EndDate=@EndDate, [Min]=@Min, [Max]=@Max,
                    IsActive=@IsActive, DaysBeforeExpiry=@DaysBeforeExpiry, GroupType=@GroupType
                WHERE Id = @Id";

            const string sqlPromo = @"
                UPDATE OfferPromoCode
                SET Nr=@Nr, PromoCode=@PromoCode, ExpiryDate=@ExpiryDate,
                    MaxUses=@MaxUses, CurrentUses=@CurrentUses, Discount=@Discount
                WHERE Id = @Id";

            using var conn = DBC.Connection;
            return offer switch
            {
                OfferVIP       => conn.Execute(sqlVip, offer),
                OfferGroup     => conn.Execute(sqlGroup, offer),
                OfferPromoCode => conn.Execute(sqlPromo, offer),
                _              => conn.Execute(sqlBase, offer)
            };
        }

        public static int DeleteOffer(int id)
        {
            const string sql = "DELETE FROM OfferBase WHERE Id = @id";
            using var conn = DBC.Connection;
            return conn.Execute(sql, new { id });
        }

        public static int ToggleActive(int id, bool isActive)
        {
            const string sql = "UPDATE OfferBase SET IsActive = @isActive WHERE Id = @id";
            using var conn = DBC.Connection;
            return conn.Execute(sql, new { id, isActive });
        }
    }
}