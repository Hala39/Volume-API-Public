using System.Threading.Tasks;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Linq;
using VAPI.Email;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using AutoMapper;

namespace VAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
        EmailSender emailSender, IMapper mapper,
        TokenService tokenService, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //tested
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null) return Unauthorized("User Not Found!");

            if (!user.EmailConfirmed) return Unauthorized("Email not confirmed!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }

            return Unauthorized("Wrong Password!");
        }

        //tested
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await _userManager.Users.AnyAsync(x => x.Email.ToLower() == registerDto.Email.ToLower() && x.EmailConfirmed == true)) 
            {
                return BadRequest("This email is already registered!");
            }

            var user = new AppUser
            {
                Email = registerDto.Email.ToLower(),
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email.ToLower()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest("Problem regestering user!");

            var origin = Request.Headers["origin"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var url = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";

            var message = 
            $"<p>Please click the below link to verify your email address:</p><p><a href='{url}'>Click to verify email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Email Verification", message);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<ActionResult<UserDto>> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized();

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) return BadRequest();

            return CreateUserObject(user);

        }

        [AllowAnonymous]
        [HttpGet("resendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized();

            var origin = Request.Headers["origin"];

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var url = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";

            var message = 
            $"<p>Please click the below link to verify your email address:</p><p><a href='{url}'>Click to verify email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Email Verification", message);


            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(FbDto fbDto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == fbDto.Email.ToLower());

            if (user != null)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }

            user = new AppUser
            {
                Email = fbDto.Email,
                DisplayName = fbDto.Name,
                ProfilePhotoUrl = fbDto.PhotoUrl,
                UserName = fbDto.Email
            };

            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }


            return BadRequest("Operation Failed.");

        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var user = await _userManager.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task SetRefreshToken(AppUser appUser)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            appUser.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(appUser);

            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookiesOptions);
        }

    }
}