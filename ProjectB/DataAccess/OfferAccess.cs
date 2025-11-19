using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace MyProject.DAL
{
    public static class OfferAccess
    {
        public static List<OfferBase> GetActiveOffers()
        {
            var now = DateTime.Now;

            const string sqlBase = @"
                SELECT Id, Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive
                FROM   OfferBase
                WHERE  IsActive = 1
                AND  @now BETWEEN StartDate AND EndDate";

            var baseOffers = DBC.Connection.Query<OfferBase>(sqlBase, new { now }).ToList();

            const string sqlVip = @"
                SELECT Id, Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive
                FROM   OfferVIP
                WHERE  IsActive = 1
                AND  @now BETWEEN StartDate AND EndDate";

            var vipOffers = DBC.Connection.Query<OfferVIP>(sqlVip, new { now })
                               .Cast<OfferBase>()
                               .ToList();

            const string sqlGroup = @"
                SELECT Id, Nr, Name, Description, Discount, StartDate, EndDate, [Min], [Max], IsActive, GroupType
                FROM   OfferGroup
                WHERE  IsActive = 1
                AND  @now BETWEEN StartDate AND EndDate";

            var groupOffers = DBC.Connection.Query<OfferGroup>(sqlGroup, new { now })
                                 .Cast<OfferBase>()
                                 .ToList();

            const string sqlPromo = @"
                SELECT Id, Nr, PromoCode, ExpiryDate, MaxUses, CurrentUses, Discount
                FROM   OfferPromoCode
                WHERE  (ExpiryDate IS NULL OR ExpiryDate >= @now)";

            var promoOffers = DBC.Connection.Query<OfferPromoCode>(sqlPromo, new { now })
                                 .Cast<OfferBase>()
                                 .ToList();

            return baseOffers.Union(vipOffers)
                              .Union(groupOffers)
                              .Union(promoOffers)
                              .ToList();
        }

        /* --------------- 写 --------------- */
        public static void IncrementPromoUse(int promoId)
        {
            const string sql = @"
                UPDATE OfferPromoCode
                SET    CurrentUses = CurrentUses + 1
                WHERE  Id = @id";

            DBC.Connection.Execute(sql, new { id = promoId });
        }
    }
}