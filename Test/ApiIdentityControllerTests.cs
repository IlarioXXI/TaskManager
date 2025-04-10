using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.models;
using Xunit;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Test
{

    public class ApiIdentityControllerTests
    {
        private readonly Mock<IValidator<AuthUser>> _validatorAuthMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<RegisterModel>> _registerValidatorMock;
        private readonly Mock<IValidator<ChangePasswordModel>> _changePasswordValidatorMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly IOptions<AppJWTSettings> _options;
        private readonly Mock<ILogger<ApiIdentityController>> _loggerMock;
        private readonly ApiIdentityController _controller;


        public ApiIdentityControllerTests()
        {

            _validatorAuthMock = new Mock<IValidator<AuthUser>>();
            _registerValidatorMock = new Mock<IValidator<RegisterModel>>();
            _changePasswordValidatorMock = new Mock<IValidator<ChangePasswordModel>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _options = Options.Create(new AppJWTSettings() { Issuer = "https://localhost:5000", Audience = "https://localhost:5000", SecretKey = "?67)5XX%14Oqaosdhiuefouefhsdhosidfhoesfhsoidfjzxkncouweh" });
            _loggerMock = new Mock<ILogger<ApiIdentityController>>();
            _controller = new ApiIdentityController(
                _options,
                _userManagerMock.Object,
                _unitOfWorkMock.Object,
                _registerValidatorMock.Object,
                _validatorAuthMock.Object,
                _changePasswordValidatorMock.Object,
                _loggerMock.Object
            );
        }


        [Fact]
        public async Task CreateJWTToken_okResultAsync()
        {
            var authUser = new AuthUser { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Id = "stringa", Email = "test@example.com"};
            
            _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUser.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUser.Password))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { SD.Role_User });


            // Act
            var result = await _controller.CreateJWTToken(authUser) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<string>(result.Value);
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
        public async Task CreateJWTToken_UnauthorizedAsync()
        {
            var authUser = new AuthUser { Email = "test@example.com", Password = "password" };
            var user = new AppUser { Id = "stringa", Email = "test@example.com" };

            _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _userManagerMock
                .Setup(um => um.FindByEmailAsync(authUser.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(user, authUser.Password))
                .ReturnsAsync(false);

            var result = await _controller.CreateJWTToken(authUser);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        }


        [Fact]
        public async Task Register_OkUser()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_User
            };

            _registerValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>()});

            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((AppUser)null);

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);


            var user = new AppUser { Id = "stringa", Email = "test@example.com" };

            _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { SD.Role_User });



            var result = await _controller.Register(registerModel);

            var okResult = Assert.IsType<OkObjectResult>(result);

        }


        [Fact]
        public async Task Register_OkAdmin()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_Admin
            };

            _registerValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((AppUser)null);

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);


            var user = new AppUser { Id = "stringa", Email = "test@example.com" };

            _validatorAuthMock
                .Setup(v => v.ValidateAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), registerModel.Password))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { SD.Role_Admin });



            var result = await _controller.Register(registerModel);

            var okResult = Assert.IsType<OkObjectResult>(result);

        }


        [Fact]
        public async void Register_BadRequest()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_Admin
            };

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

            var result = await _controller.Register(registerModel);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async void Register_BadRequest_UserExists()
        {
            var registerModel = new RegisterModel()
            {
                Email = "email@email.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = SD.Role_Admin
            };
            _registerValidatorMock
               .Setup(v => v.ValidateAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult
               {
                   Errors = new List<ValidationFailure>()
               });
            _unitOfWorkMock
                .Setup(uow => uow.AppUser.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AppUser());

            var result = await _controller.Register(registerModel);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User already registered!", badRequestResult.Value);
        }
    }
}

