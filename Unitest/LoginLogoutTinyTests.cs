using Moq;
using Xunit;

namespace MyProject.Tests
{
    public class LoginLogoutTinyTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<ISessionService> _session = new();

        [Fact]
        public void L01_WrongPwd_ReturnsNull()
        {
            _userRepo.Setup(r => r.GetByAccount("bob")).Returns(new User { Password = "right" });
            var logic = new LoginLogic(_userRepo.Object, _session.Object);
            Assert.Null(logic.Authenticate("bob", "wrong"));
        }

        [Fact]
        public void L02_RightPwd_ReturnsUser()
        {
            var user = new User { Password = "right" };
            _userRepo.Setup(r => r.GetByAccount("bob")).Returns(user);
            var logic = new LoginLogic(_userRepo.Object, _session.Object);
            Assert.Equal(user, logic.Authenticate("bob", "right"));
        }


        [Fact]
        public void LO01_Logout_ClearsSession()
        {
            var logic = new LogoutLogic(_session.Object);
            logic.Logout();
            _session.Verify(s => s.ClearCurrentUser(), Times.Once);
        }
    }
}