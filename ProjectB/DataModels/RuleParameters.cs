using Newtonsoft.Json;

namespace ProjectB.DataModels
{
    public class AgeRuleParameters
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }

    public class QuantityRuleParameters
    {
        public int MinQuantity { get; set; }
    }

    public class PromoCodeRuleParameters
    {
        public string Code { get; set; } = "";
        public DateTime? ExpiryDate { get; set; }
        public int? MaxUses { get; set; }
        public int CurrentUses { get; set; }
    }

    public class BirthdayRuleParameters
    {
        public int DaysBefore { get; set; } = 7;
        public int DaysAfter { get; set; } = 7;
    }

    public static class OfferTypes
    {
        public const string Age = "Age";
        public const string Quantity = "Quantity";
        public const string PromoCode = "PromoCode";
        public const string Birthday = "Birthday";
    }
}
