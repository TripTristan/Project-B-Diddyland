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
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sqlBase = "SELECT * FROM OfferBase WHERE IsActive = 1";
                const string sqlVip  = "SELECT * FROM OfferVIP  WHERE IsActive = 1";
                const string sqlGroup= "SELECT * FROM OfferGroup WHERE IsActive = 1";
                const string sqlPromo= "SELECT * FROM OfferPromoCode WHERE (ExpiryDate IS NULL OR ExpiryDate >= datetime('now'))";

                var baseList = DBC.Connection.Query<OfferBase>(sqlBase).ToList();
                var vipList  = DBC.Connection.Query<OfferVIP>(sqlVip).Cast<OfferBase>().ToList();
                var grpList  = DBC.Connection.Query<OfferGroup>(sqlGroup).Cast<OfferBase>().ToList();
                var proList  = DBC.Connection.Query<OfferPromoCode>(sqlPromo).Cast<OfferBase>().ToList();

                return baseList.Union(vipList).Union(grpList).Union(proList).ToList();
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static OfferBase? GetOfferById(int id)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                var baseOffer = DBC.Connection.QueryFirstOrDefault<OfferBase>("SELECT * FROM OfferBase WHERE Id = @id", new { id });
                if (baseOffer != null)
                    return baseOffer;

                var vipOffer = DBC.Connection.QueryFirstOrDefault<OfferVIP>("SELECT * FROM OfferVIP WHERE Id = @id", new { id });
                if (vipOffer != null)
                    return vipOffer;

                var groupOffer = DBC.Connection.QueryFirstOrDefault<OfferGroup>("SELECT * FROM OfferGroup WHERE Id = @id", new { id });
                if (groupOffer != null)
                    return groupOffer;

                return DBC.Connection.QueryFirstOrDefault<OfferPromoCode>("SELECT * FROM OfferPromoCode WHERE Id = @id", new { id });
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        /* --------------- 增 --------------- */
        public static int InsertOffer(OfferBase offer)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sqlBase = @"
                    INSERT INTO OfferBase (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry)
                    VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry);
                    SELECT last_insert_rowid();";

                const string sqlVip = @"
                    INSERT INTO OfferVIP (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry)
                    VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry);
                    SELECT last_insert_rowid();";

                const string sqlGroup = @"
                    INSERT INTO OfferGroup (Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, DaysBeforeExpiry, GroupType)
                    VALUES (@Nr, @Name, @Description, @Discount, @StartDate, @EndDate, @Min, @Max, @IsActive, @DaysBeforeExpiry, @GroupType);
                    SELECT last_insert_rowid();";

                const string sqlPromo = @"
                    INSERT INTO OfferPromoCode (Nr, PromoCode, ExpiryDate, MaxUses, CurrentUses, Discount)
                    VALUES (@Nr, @PromoCode, @ExpiryDate, @MaxUses, @CurrentUses, @Discount);
                    SELECT last_insert_rowid();";

                return offer switch
                {
                    OfferVIP     => DBC.Connection.QuerySingle<int>(sqlVip, offer),
                    OfferGroup   => DBC.Connection.QuerySingle<int>(sqlGroup, offer),
                    OfferPromoCode => DBC.Connection.QuerySingle<int>(sqlPromo, offer),
                    _            => DBC.Connection.QuerySingle<int>(sqlBase, offer)
                };
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        /* --------------- 改 --------------- */
        public static int UpdateOffer(OfferBase offer)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

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

                return offer switch
                {
                    OfferVIP       => DBC.Connection.Execute(sqlVip, offer),
                    OfferGroup     => DBC.Connection.Execute(sqlGroup, offer),
                    OfferPromoCode => DBC.Connection.Execute(sqlPromo, offer),
                    _              => DBC.Connection.Execute(sqlBase, offer)
                };
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static int DeleteOffer(int id)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = "DELETE FROM OfferBase WHERE Id = @id";
                return DBC.Connection.Execute(sql, new { id });
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }

        public static int ToggleActive(int id, bool isActive)
        {
            try
            {
                if (DBC.Connection.State != System.Data.ConnectionState.Open)
                    DBC.Connection.Open();

                const string sql = "UPDATE OfferBase SET IsActive = @isActive WHERE Id = @id";
                return DBC.Connection.Execute(sql, new { id, isActive });
            }
            finally
            {
                if (DBC.Connection.State == System.Data.ConnectionState.Open)
                    DBC.Connection.Close();
            }
        }
    }
}