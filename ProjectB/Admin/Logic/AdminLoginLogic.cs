using ProjectB.DataModels;
using ProjectB.Customer.DataAccess;

namespace ProjectB.Admin.Logic
{
    public class AdminLoginLogic
    {
        private readonly UserAccess _userAccess;

        public AdminLoginLogic(UserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public bool AuthenticateAdmin(string username, string password)
        {
            var admin = _userAccess.GetAdminByUsername(username);
            if (admin == null) return false;
            if (admin.Password != password) return false;

            LoginStatus.Login(admin);
            return true;
        }

        public static bool IsAdminLoggedIn()
        {
            return LoginStatus.CurrentUserInfo != null && 
                   LoginStatus.CurrentUserInfo.Role == "Admin";
        }
    }
}
