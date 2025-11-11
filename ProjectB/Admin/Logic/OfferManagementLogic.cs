public class OfferManagementLogic
{
    private readonly OfferAccess _dataAccess;
    
    public OfferService()
    {
        _dataAccess = new OfferAccess();
    }

    public List<OfferModel> GetAllOffers()
    {
        return _dataAccess.GetAll().ToList();
    }

    public OfferModel GetOfferById(int id)
    {
        return _dataAccess.GetById(id);
    }

    public Result AddOffer(OfferModel offer)
    {
        var validation = ValidateOffer(offer);
        if (!validation.IsSuccess)
            return validation;

        try
        {
            _dataAccess.Insert(offer);
            return Result.Success("Offer added successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to add offer: {ex.Message}");
        }
    }

    public Result UpdateOffer(int id, OfferModel offer)
    {
        var existing = _dataAccess.GetById(id);
        if (existing == null)
            return Result.Fail("Offer not found.");

        var validation = ValidateOffer(offer);
        if (!validation.IsSuccess)
            return validation;

        try
        {
            offer.Id = id;
            _dataAccess.Update(offer);
            return Result.Success("Offer updated successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update offer: {ex.Message}");
        }
    }

    public Result ToggleOfferStatus(int id)
    {
        var offer = _dataAccess.GetById(id);
        if (offer == null)
            return Result.Fail("Offer not found.");

        try
        {
            _dataAccess.SetActive(id, !offer.IsActive);
            return Result.Success($"Offer {(offer.IsActive ? "deactivated" : "activated")} successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to toggle offer: {ex.Message}");
        }
    }

    public Result DeleteOffer(int id)
    {
        var offer = _dataAccess.GetById(id);
        if (offer == null)
            return Result.Fail("Offer not found.");

        try
        {
            _dataAccess.Delete(id);
            return Result.Success("Offer deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to delete offer: {ex.Message}");
        }
    }

    private Result ValidateOffer(OfferModel offer)
    {
        if (string.IsNullOrWhiteSpace(offer.Name))
            return Result.Fail("Name is required.");

        if (offer.Discount < 0 || offer.Discount > 1)
            return Result.Fail("Discount must be between 0 and 100%.");

        if (offer.StartDate > offer.EndDate)
            return Result.Fail("Start date cannot be later than end date.");

        if (offer.Rules == null || !offer.Rules.Any())
            return Result.Fail("At least one rule is required.");

        return Result.Success();
    }
}


public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static Result Success(string message = null)
    {
        return new Result { IsSuccess = true, Message = message };
    }

    public static Result Fail(string message)
    {
        return new Result { IsSuccess = false, Message = message };
    }
}