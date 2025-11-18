using Xunit;
using ProjectB.DataModels;
using System;
using static ProjectB.Customer.LoginStatus;

namespace ProjectB.Customer.Logic.Tests
{
    public class LoginStatusTests : IDisposable
    {
        private readonly UserModel _testUser1 = new UserModel(
            id: 1,
            name: "TestUser1",
            email: "test1@example.com",
            dateOfBirth: "01/01/1990",
            height: 170,
            phone: "1234567890",
            password: "password123"
        );

        private readonly UserModel _testUser2 = new UserModel(
            id: 2,
            name: "TestUser2",
            email: "test2@example.com",
            dateOfBirth: "02/02/1995",
            height: 165,
            phone: "0987654321",
            password: "password456"
        );

        public LoginStatusTests()
        {
            Logout();
        }

        public void Dispose()
        {
            Logout();
        }

        [Fact]
        public void CurrentUserInfo_Initially_ShouldBeNull()
        {
            Logout();
            Assert.Null(CurrentUserInfo);
        }

        [Fact]
        public void Login_WithValidUser_ShouldSetCurrentUser()
        {
            Login(_testUser1);

            Assert.NotNull(CurrentUserInfo);
            Assert.Equal(_testUser1.Id, CurrentUserInfo?.Id);
            Assert.Equal(_testUser1.Name, CurrentUserInfo?.Name);
        }

        [Fact]
        public void Logout_WhenUserIsLoggedIn_ShouldClearCurrentUser()
        {
            Login(_testUser1);
            Assert.NotNull(CurrentUserInfo);
            
            Logout();
            
            Assert.Null(CurrentUserInfo);
        }

        [Fact]
        public void MultipleLoginLogout_ShouldWorkCorrectly()
        {
            Login(_testUser1);
            Assert.Equal(_testUser1.Id, CurrentUserInfo?.Id);
            
            Logout();
            Assert.Null(CurrentUserInfo);
            
            Login(_testUser2);
            Assert.Equal(_testUser2.Id, CurrentUserInfo?.Id);
        }

        [Fact]
        public void Login_WithNullUser_ShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => Login(null));
            Assert.Equal("Value cannot be null. (Parameter 'accountInfo')", exception.Message);
        }
    }
}
