public class UserUpdateLogic
    {
        public UserModel? GetById(int id) => UserAccess.GetById(id);

        public (bool ok, string? error) UpdateProfile(UserModel updated)
        {
            // Validate with UserLogic
            if (!UserLogic.IsNameValid(updated.Name))
                return (false, "Invalid name. It must be 3–19 characters and contain no digits.");

            if (!UserLogic.IsEmailValid(updated.Email))
                return (false, "Invalid email format.");

            if (!UserLogic.IsDateOfBirthValid(updated.DateOfBirth))
                return (false, "Invalid date of birth. Use dd-mm-yyyy and a real calendar date.");

            if (!UserLogic.IsHeightValid(updated.Height))
                return (false, "Invalid height. Enter a value between 30 and 250 cm.");

            if (!UserLogic.IsPhoneValid(updated.Phone))
                return (false, "Invalid phone. Use +########### (12 chars) or Dutch 06######## (10 chars).");

            if (!UserLogic.IsPasswordValid(updated.Password))
                return (false, "Invalid password. 8–16 chars with upper, lower, digit, and special.");

            try
            {
                UserAccess.Update(updated);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Could not save changes: {ex.Message}");
            }
        }
    }