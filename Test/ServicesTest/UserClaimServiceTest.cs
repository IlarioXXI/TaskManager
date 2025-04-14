using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManager.Services.Services;
using Xunit;

namespace Test.ServicesTest
{
    public class UserClaimServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;
        private readonly UserClaimService _userClaimService;

        public UserClaimServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _userClaimService = new UserClaimService(_httpContextMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void GetClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("ClaimType1", "ClaimValue1"),
                new Claim("ClaimType2", "ClaimValue2")
            };
            _httpContextMock.Setup(h => h.HttpContext.User.Claims).Returns(claims.AsQueryable());
            // Act
            var result = _userClaimService.GetClaims();
            // Assert
            Assert.Equal(claims, result);
        }

        [Fact]
        public void GetUserId()
        {
            // Arrange
            var userId = "test-user-id";
            _httpContextMock.Setup(h => h.HttpContext.User.Identity.Name).Returns(userId);
            // Act
            var result = _userClaimService.GetUserId();
            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public void Getuser()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new AppUser { Id = userId, UserName = "test-user-name" };
            _httpContextMock.Setup(h => h.HttpContext.User.Identity.Name).Returns(userId);
            _unitOfWorkMock.Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(user);
            // Act
            var result = _userClaimService.GetUser();
            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public void GetUserEmail()
        {
            // Arrange
            var email = "test@email.com";
            _httpContextMock.Setup(h => h.HttpContext.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                })));
            // Act
            var result = _userClaimService.GetUSerEmail();
            // Assert
            Assert.Equal(email, result);
        }
    }
}
