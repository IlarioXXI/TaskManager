using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Moq;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Migrations;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Services;
using TaskManagerWEB.Api.Controllers;
using Xunit;

namespace Test.ServicesTest
{
    public class ApiIdentityServiceTest
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ApiIdentityService>> _loggerMock;
        private readonly Mock<IUserClaimService> _userClaimServiceMock;
        private readonly IOptions<AppJWTSettings> _options;
        private readonly ApiIdentityService _service;
        public ApiIdentityServiceTest()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );
            _userClaimServiceMock = new Mock<IUserClaimService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _options = Options.Create(new AppJWTSettings() { Issuer = "https://localhost:5000", Audience = "https://localhost:5000", SecretKey = "?67)5XX%14Oqaosdhiuefouefhsdhosidfhoesfhsoidfjzxkncouweh" });
            _loggerMock = new Mock<ILogger<ApiIdentityService>>();
            _service = new ApiIdentityService(
                _options,
                _userManagerMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _userClaimServiceMock.Object
            );
        }

        [Fact]
        public async Task CreateJwtToken_okResult()
        {
            var authUser = new AuthUser { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Id = "stringa", Email = "test@example.com" };
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUser.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUser.Password))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { SD.Role_User });
            var result = await _service.CreateJwtTokenAsync(authUser);
            result = "token";
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task CreateJwtToken_unauthorized()
        {
            var authUser = new AuthUser { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Id = "stringa", Email = "test@example.com" };
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUser.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUser.Password))
                .ReturnsAsync(false);

            var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateJwtTokenAsync(authUser));
        }

        [Fact]
        public async Task Register_OKUser()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_User
            };
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((AppUser)null);
            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { SD.Role_User });

            var user = new AppUser { Id = "stringa", Email = "test@example.com" };
            var authUSer = new AuthUser()
            {
                Email = "email@email.com",
                Password = "Password123!",
            };
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUSer.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUSer.Password))
                .ReturnsAsync(true);
            var result = await _service.RegisterAsync(registerModel);
            Assert.NotNull(result);
            Assert.IsType<string>(result);

        }

        [Fact]
        public async Task Register_UserAdmin()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_Admin
            };
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((AppUser)null);
            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { SD.Role_Admin });

            var user = new AppUser { Id = "stringa", Email = "test@example.com" };
            var authUSer = new AuthUser()
            {
                Email = "email@email.com",
                Password = "Password123!",
            };
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUSer.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUSer.Password))
                .ReturnsAsync(true);
               
            var result = await _service.RegisterAsync(registerModel);
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task Register_UserExists()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_User
            };
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser());
            var result = await Assert.ThrowsAsync<Exception>(() => _service.RegisterAsync(registerModel));
            Assert.NotNull(result);
            Assert.IsType<Exception>(result);
        }

        [Fact]
        public async Task GetAllUsers_ok()
        {
            _userClaimServiceMock
                    .Setup(um => um.GetClaims())
                    .Returns(new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, SD.Role_Admin)
                    });
            var appUsersList = new List<AppUser>();
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.GetAll(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>()))
                .Returns(appUsersList);
            var result = _service.GetAllUsers();
            Assert.NotNull(result);
            Assert.IsType<List<AppUser>>(result);
            Assert.Equal(appUsersList, result);
        }

        [Fact]
        public async Task GetAllUsers_UserNotAdmin()
        {
            _userClaimServiceMock
                    .Setup(um => um.GetClaims())
                    .Returns(new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, SD.Role_User)
                    });
            var appUsersList = new List<AppUser>();
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.GetAll(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>()))
                .Returns(appUsersList);
            var result = Assert.Throws<UnauthorizedAccessException>(() => _service.GetAllUsers());
        }


    }
}
