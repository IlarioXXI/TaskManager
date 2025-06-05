using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Interfaces;

namespace TaskManager.Services.Services
{
    public class ApiIdentityService : IApiIdentityService
    {
        private readonly string JWT_ISSUER;
        private readonly string JWT_AUDIENCE;
        private readonly byte[] JWT_SECRETKEY;

        private const string JWT_TOKEN_ID = "id";
        private const int JWT_EXPIRATION_MINUTES = 15;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApiIdentityService> _logger;
        private readonly IUserClaimService _userClaimService;



        public ApiIdentityService(IOptions<AppJWTSettings> settings,
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<ApiIdentityService> logger,
            IUserClaimService userClaimService)
        {
            JWT_ISSUER = settings.Value.Issuer;
            JWT_AUDIENCE = settings.Value.Audience;
            JWT_SECRETKEY = Encoding.UTF8.GetBytes(settings.Value.SecretKey);
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userClaimService = userClaimService;
        }


        public async Task<string> CreateJwtTokenAsync(string email,string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogInformation("User {email} tried to login with wrong password", email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            DateTime expiration = DateTime.UtcNow.AddMinutes(JWT_EXPIRATION_MINUTES);

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = false;
            if (userRoles.Contains(SD.Role_Admin))
            {
                isAdmin = true;
            }

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (JWT_TOKEN_ID, Guid.NewGuid().ToString()),
                    //new (JwtRegisteredClaimNames.NameId, authUser.AppUserId),
                    new (ClaimTypes.Role,isAdmin ? SD.Role_Admin : SD.Role_User),
                    new (JwtRegisteredClaimNames.Sub, user.Id),
                    new (ClaimTypes.Name, user.Id),
                    new (JwtRegisteredClaimNames.Name, email),
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
            _logger.LogInformation("User {email} is logged at : {data}", email, DateTime.Now.ToLocalTime());
            return tokenString;
        }

        public IEnumerable<AppUser> GetAllUsers()
        {
            var userRole = _userClaimService.GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (userRole != SD.Role_Admin)
            {
                throw new UnauthorizedAccessException("You are not authorized to view all users");
            }
            ;
            var users = _unitOfWork.AppUser.GetAll();
            return users;
        }

        public async Task<string> RegisterAsync(string email, string passWord, string roleToAdd)
        {
            var existingUser = _unitOfWork.AppUser.Get(u => u.UserName == email);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }
            var user = new AppUser
            {
                UserName = email,
                Email = email,
            };
            var result = await _userManager.CreateAsync(user, passWord);

            await _userManager.AddToRoleAsync(user, roleToAdd);
            var role = await _userManager.GetRolesAsync(user);

            var token = await CreateJwtTokenAsync(email,passWord);

            return token;
        }

        public async Task<bool> MyChangePasswordAsync(string currentPass, string newPass)
        {
            var user = _userClaimService.GetUserTracked();
            if (user == null)
            {
                _logger.LogWarning("User not found when attempting to change password.");
                throw new Exception("User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPass, newPass);
            if (!result.Succeeded)
            {
                _logger.LogInformation("User {email} changed is password", user.Email);
                throw new Exception("Password not changed");
            }
            _logger.LogInformation("User {email} changed is password", user.Email);
            return true;
        }

        public string getEmail()
        {
            var user = _unitOfWork.AppUser.Get(x => x.Id == _userClaimService.GetUserId());
            return user.Email;
        }
    }
}
