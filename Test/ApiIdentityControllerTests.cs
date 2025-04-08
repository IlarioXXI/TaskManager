using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.models;
using Xunit;

namespace Test
{

    public class ApiIdentityControllerTests
    {
        private readonly Mock<IValidator<AuthUser>> _validatorMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IOptions<AppJWTSettings>> _optionsMock;
        private readonly Mock<ILogger<ApiIdentityController>> _loggerMock;
        private readonly ApiIdentityController _controller;

    }
}

