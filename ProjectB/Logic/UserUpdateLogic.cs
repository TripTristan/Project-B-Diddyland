using System;

public class UserUpdateLogic
{
    private readonly UserAccess _userAccess;
    private readonly UserLogic _userLogic;

    public UserUpdateLogic(UserAccess userAccess, UserLogic userLogic)
    {
        _userAccess = userAccess;
        _userLogic = userLogic;
    }

    public UserModel? GetById(int id) => _userAccess.GetById(id);

    public (bool ok, string? error) UpdateProfile(UserModel updated)
    {
        if (!_userLogic.IsNameValid(updated.Name))
            return (false, "Invalid name. It must be 3–19 characters and contain no digits.");

        if (!_userLogic.IsEmailValid(updated.Email))
            return (false, "Invalid email format.");

        if (!_userLogic.IsDateOfBirthValid(updated.DateOfBirth))
            return (false, "Invalid date of birth. Use dd-mm-yyyy and a real calendar date.");

        if (!_userLogic.IsHeightValid(updated.Height))
            return (false, "Invalid height. Enter a value between 30 and 250 cm.");

        if (!_userLogic.IsPhoneValid(updated.Phone))
            return (false, "Invalid phone. Use +########### (12 chars) or Dutch 06######## (10 chars).");

        if (!_userLogic.IsPasswordValid(updated.Password))
            return (false, "Invalid password. 8–16 chars with upper, lower, digit, and special.");

        try
        {
            _userAccess.Update(updated);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Could not save changes: {ex.Message}");
        }
    }
}
