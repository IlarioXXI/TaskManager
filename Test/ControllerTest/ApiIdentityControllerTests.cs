using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.ContentModel;
using System.Linq.Expressions;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManagerWeb.Api.ViewModels.UserViewModels;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.ViewModels.UserViewModels;
using Xunit;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Test.ControllerTest
{

    public class ApiIdentityControllerTests
    {
        private readonly Mock<IValidator<RegisterModel>> _registerValidatorMock;
        private readonly Mock<IValidator<AuthUser>> _validatorAuthMock;
        private readonly Mock<IValidator<ChangePasswordModel>> _validatorPassMock;
        private readonly Mock<IApiIdentityService> _apiIdentityServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ApiIdentityController _controller;
        public ApiIdentityControllerTests()
        {
            _registerValidatorMock = new Mock<IValidator<RegisterModel>>();
            _validatorAuthMock = new Mock<IValidator<AuthUser>>();
            _validatorPassMock = new Mock<IValidator<ChangePasswordModel>>();
            _mapperMock = new Mock<IMapper>();
            _apiIdentityServiceMock = new Mock<IApiIdentityService>();
            _controller = new ApiIdentityController(
                _registerValidatorMock.Object,
                _validatorAuthMock.Object,
                _validatorPassMock.Object,
                _apiIdentityServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task CreateJWTToken_okResultAsync()
        {
           _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _apiIdentityServiceMock
                .Setup(a => a.CreateJwtTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("token");
            var result = await _controller.CreateJWTToken(new AuthUser());
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("token", okResult.Value);
        }


        [Fact]
        public async Task CreateJWTToken_BadRequest()
        {
            _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult
                {
                    Errors = new List<ValidationFailure>()
                {
                    new ValidationFailure("Email", "Invalid email format"),
                    new ValidationFailure("Password", "Password is required")
                }
                });
            var result = await _controller.CreateJWTToken(new AuthUser());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task Register_Ok()
        {
            var registerUser = new RegisterModel()
            {
                Email = "test@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_User
            };
            _registerValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _apiIdentityServiceMock
                .Setup(a => a.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("token");
            var result = await _controller.Register(new RegisterModel());
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("token", okResult.Value);

        }


        [Fact]
        public async void Register_BadRequest()
        {

            _registerValidatorMock
               .Setup(v => v.ValidateAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult
               {
                   Errors = new List<ValidationFailure>()
               {
                   new ValidationFailure("Email", "Invalid email format"),
                   new ValidationFailure("Password", "Password is required")
               }
               });
            var result = await _controller.Register(It.IsAny<RegisterModel>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public void GetAllUseers_ok()
        {
            var list = new List<AppUser>()
            {
                new AppUser() { Email = "email@email.com", Id= "1", UserName = "User1" },
                new AppUser() {Email = "email2@email.com", Id= "2", UserName = "User2" }
            };
            _apiIdentityServiceMock
                .Setup(a => a.GetAllUsers())
                .Returns(list);
            var result = _controller.GetAllUsers();
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassAsync_okAsync()
        {
            _validatorPassMock
                .Setup(v => v.ValidateAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _apiIdentityServiceMock
                .Setup(a => a.MyChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            var model = new ChangePasswordModel()
            {
                CurrentPassword = "string",
                NewPassword = "String",
                ConfirmNewPassword = "String"
            };
            var result = await _controller.ChangePasswordAsync(model);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
        }

        [Fact]
        public async Task ChangePassAsync_BadRequest()
        {
            _validatorPassMock
                .Setup(v => v.ValidateAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult
                {
                    Errors = new List<ValidationFailure>()
                {
                    new ValidationFailure("CurrentPassword", "Invalid email format"),
                    new ValidationFailure("NewPassword", "Password is required")
                }
                });
            var result = await _controller.ChangePasswordAsync(It.IsAny<ChangePasswordModel>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

