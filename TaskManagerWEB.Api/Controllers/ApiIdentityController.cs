using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;
using TaskManager.Services.Extensions;
using TaskManager.Services.Interfaces;
using TaskManagerWEB.Api.ViewModels.UserViewModels;

namespace TaskManagerWEB.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiIdentityController : Controller
    {

        private readonly IValidator<RegisterModel> _validator;
        private readonly IValidator<AuthUser> _validatorUser;
        private readonly IValidator<ChangePasswordModel> _validatorPass;
        private readonly IApiIdentityService _apiIdentityService;
        private readonly IMapper _mapper;



        public ApiIdentityController(
            IValidator<RegisterModel> validator,
            IValidator<AuthUser> validatorUser,
            IValidator<ChangePasswordModel> validatorPass,
            IApiIdentityService apiIdentityService,
            IMapper mapper)
        {
            _validator = validator;
            _validatorUser = validatorUser;
            _validatorPass = validatorPass;
            _apiIdentityService = apiIdentityService;
            _mapper = mapper;

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

            var result = await _apiIdentityService.CreateJwtTokenAsync(authUser);
            return Ok(result);
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
            var result = await _apiIdentityService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpGet("getAllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAllUsers()
        {
            var result = _apiIdentityService.GetAllUsers();
            var resultVM = _mapper.Map<IEnumerable<AppUser>, IEnumerable<AppUserVM>>(result);
            return Ok(resultVM);
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
            
            var result = await _apiIdentityService.MyChangePasswordAsync(model);
            return Ok(result);
        }

    }

}
