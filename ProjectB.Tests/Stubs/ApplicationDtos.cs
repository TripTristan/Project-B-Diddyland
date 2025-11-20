public class SessionAvailabilityDto
{
    public int Id { get; set; }
    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public decimal BasisPrice { get; set; }
    public int AvailableSeats { get; set; }
}

public class GroupReservationDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public GroupType GroupType { get; set; }
    public string OrganizationName { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string ContactPhone { get; set; } = "";
    public int GroupSize { get; set; }
    public int SessionId { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
}

public class OfferDto
{
    public int Id { get; set; }
    public string Nr { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Discount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public int? DaysBeforeExpiry { get; set; }
    public OfferType Type { get; set; } = OfferType.Age;
    public GroupType? GroupType { get; set; }
    public string? PromoCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MaxUses { get; set; }
    public int? CurrentUses { get; set; }
}

public class OfferValidationResultDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

public class OfferListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Discount { get; set; }
    public bool IsActive { get; set; }
    public string Type { get; set; } = "";
}

