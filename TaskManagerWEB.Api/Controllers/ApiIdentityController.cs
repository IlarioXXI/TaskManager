using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManagerWEB.Api.models;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiIdentityController : Controller
    {
        private readonly string JWT_ISSUER;
        private readonly string JWT_AUDIENCE;
        private readonly byte[] JWT_SECRETKEY;

        private const string JWT_TOKEN_ID = "id";
        private const int JWT_EXPIRATION_MINUTES = 15;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterModel> _validator;
        private readonly IValidator<AuthUser> _validatorUser;
        private readonly IValidator<ChangePasswordModel> _validatorPass;
        private readonly ILogger<ApiIdentityController> _logger;



        public ApiIdentityController(IOptions<AppJWTSettings> settings,
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork,
            IValidator<RegisterModel> validator,
            IValidator<AuthUser> validatorUser,
            IValidator<ChangePasswordModel> validatorPass,
            ILogger<ApiIdentityController> logger)
        {
            JWT_ISSUER = settings.Value.Issuer;
            JWT_AUDIENCE = settings.Value.Audience;
            JWT_SECRETKEY = Encoding.UTF8.GetBytes(settings.Value.SecretKey);
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validatorUser = validatorUser;
            _validatorPass = validatorPass;
            _logger = logger;
        }

        [Route("auth")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateJWTToken(
            [FromBody] AuthUser authUser)
        {
            var resultValidation = await _validatorUser.ValidateAsync(authUser);
            if (!resultValidation.IsValid)
            {
                resultValidation.AddToModelState(this.ModelState);
                var errors = new List<string>();
                foreach (var error in resultValidation.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                return BadRequest(errors);
            }
            var user = await _userManager.FindByEmailAsync(authUser.Email);
            if (!await _userManager.CheckPasswordAsync(user, authUser.Password))
            {
                _logger.LogInformation("User {email} tried to login with wrong password", authUser.Email);
                return Unauthorized("Wrong credentials!");
            }

            DateTime expiration = DateTime.UtcNow.AddMinutes(JWT_EXPIRATION_MINUTES);

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = false;
            if (userRoles.Contains(SD.Role_Admin))
            {
                isAdmin = true;
            }

            //var user = AuthenticateUser(authUser.UserName, authUser.Password);
            // configure and send final token.

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (JWT_TOKEN_ID, Guid.NewGuid().ToString()),
                    //new (JwtRegisteredClaimNames.NameId, authUser.AppUserId),
                    new (ClaimTypes.Role,isAdmin ? SD.Role_Admin : SD.Role_User),
                    new (JwtRegisteredClaimNames.Sub, user.Id),
                    new (ClaimTypes.Name, user.Id),
                    new (JwtRegisteredClaimNames.Name, authUser.Email),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        }),
                SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(JWT_SECRETKEY), SecurityAlgorithms.HmacSha256Signature),
                Expires = expiration,
                Issuer = JWT_ISSUER,
                Audience = JWT_AUDIENCE,
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descriptor);
            string tokenString = handler.WriteToken(token);
            _logger.LogInformation("User {email} is logged at : {data}", authUser.Email, DateTime.Now.ToLocalTime());
            return Ok(tokenString);
        }


        [HttpPost("Register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var resultValidation = await _validator.ValidateAsync(model);
            if (!resultValidation.IsValid)
            {
                resultValidation.AddToModelState(this.ModelState);
                var errors = new  List<string>();
                foreach (var error in resultValidation.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                return BadRequest(errors);
            }
            var users = _unitOfWork.AppUser.GetAll();
            var existingUser = _unitOfWork.AppUser.Get(u => u.UserName == model.Email);
            if (existingUser != null)
            {
                return BadRequest("User already registered!");
            }
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user,model.Password);
           
            await _userManager.AddToRoleAsync(user, model.Role);
            var role = await _userManager.GetRolesAsync(user);

            var token = await CreateJWTToken(new AuthUser
            {
                Email = model.Email,
                Password = model.Password
            });

            if (token is OkObjectResult okResult)
            {
                var tokenToString = okResult.Value as string;
                return Ok(tokenToString);
            }

            return BadRequest("Invalid credentials or other error.");
        }

        [HttpGet("getAllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAllUsers()
        {
            if (!User.IsInRole(SD.Role_Admin))
            {
                return Unauthorized();
            };
            var users = _unitOfWork.AppUser.GetAll();
            return Ok(users);
        }


        [HttpPost("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordModel model)
        {
            var resultValidation = await _validatorPass.ValidateAsync(model);
            if (!resultValidation.IsValid)
            {
                resultValidation.AddToModelState(this.ModelState);
                var errors = new List<string>();
                foreach (var error in resultValidation.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                return BadRequest(errors);
            }
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user,model.CurrentPassword,model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {email} changed is password",user.Email);
                return Ok($"{user.Email}: password changed!");
            }
            return BadRequest(result.Errors.FirstOrDefault().Description);
        }

    }
}
