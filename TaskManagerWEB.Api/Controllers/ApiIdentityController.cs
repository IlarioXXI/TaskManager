using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public ApiIdentityController(IOptions<AppJWTSettings> settings)
        {
            JWT_ISSUER = settings.Value.Issuer;
            JWT_AUDIENCE = settings.Value.Audience;
            JWT_SECRETKEY = Encoding.UTF8.GetBytes(settings.Value.SecretKey);
        }
        [Route("auth")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateJWTToken(
            [FromBody]AuthUser authUser )
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(JWT_EXPIRATION_MINUTES);

            // configure and send final token.

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new (JWT_TOKEN_ID, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, authUser.UserName),
            new (JwtRegisteredClaimNames.Email, authUser.UserName),
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

            return Ok(tokenString);
        }
    }
}
