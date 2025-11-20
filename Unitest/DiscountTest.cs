using System;
using System.Collections.Generic;
using MyProject.BLL;
using MyProject.Models;
using Xunit;

namespace MyProject.Tests
{
    public class DiscountTests
    {
        // Fact is call for Test Runner, hier is a test method
        [Fact] public void B1_BirthdayFirstFree()        => Assert.Equal(0, ApplyBirthday().Details[0].FinalPrice);
        [Fact] public void B2_BirthdaySecondFullPrice()  => Assert.Equal(15, ApplyBirthday().Details[1].FinalPrice);

        [Fact] public void V1_Vip5Off() => Assert.Equal(15*0.95m, ApplyVIP().Details[0].FinalPrice);

        [Fact] public void G1_School20Off() => Assert.Equal(15*0.80m, ApplySchool().Details[0].FinalPrice);
        [Fact] public void G2_Comp15Off()   => Assert.Equal(15*0.85m, ApplyCompany().Details[0].FinalPrice);

        [Fact] public void P1_Promo10Off() => Assert.Equal(15*0.90m, ApplyPromo().Details[0].FinalPrice);

        private static DiscountSummaryDto ApplyBirthday()
        {
            var cart = new List<(int, int)> { (1, 30), (2, 25) };
            var cust = new UserModel{ Id=1, DateOfBirth="15-06-2000" };
            return DiscountLogic.ApplyAllDiscounts(cart, cust, null, new DateTime(2025,6,17));
        }
        private static DiscountSummaryDto ApplyVIP()
        {
            var cart = new List<(int, int)> { (1, 30) };
            var cust = new UserModel{ Level = UserLevel.VIP };
            return DiscountLogic.ApplyAllDiscounts(cart, cust);
        }
        private static DiscountSummaryDto ApplySchool()
        {
            var cart = new List<(int, int)>();
            for(int i=0;i<20;i++) cart.Add((i,12));
            return DiscountLogic.ApplyAllDiscounts(cart, new UserModel());
        }
        private static DiscountSummaryDto ApplyCompany()
        {
            var cart = new List<(int, int)>();
            for(int i=0;i<10;i++) cart.Add((i,40));
            return DiscountLogic.ApplyAllDiscounts(cart, new UserModel());
        }
        private static DiscountSummaryDto ApplyPromo()
        {
            var cart = new List<(int, int)> { (1, 30) };
            return DiscountLogic.ApplyAllDiscounts(cart, new UserModel(), "SAVE10");
        }
    }
}