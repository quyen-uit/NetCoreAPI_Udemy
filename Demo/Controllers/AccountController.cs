using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
             _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://graph.facebook.com")
            };
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(x=> x.Photos)
                .FirstOrDefaultAsync(x=> x.Email ==loginDto.Email);

            if (user == null) return Unauthorized();
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateAppUser(user);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email existed");
                return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                ModelState.AddModelError("username", "User Name existed.");
                return ValidationProblem();
            }

            AppUser user = new AppUser
            {
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return CreateAppUser(user);
            }
            else
            {
                ModelState.AddModelError("registererror", "Register error:" + result.Errors.ToList()[0].Description);
                return ValidationProblem();
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users.Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            await SetRefreshToken(user);
            return CreateAppUser(user);
        }

        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> LoginFacebook(string accessToken)
        {
            var verifyKeys = _configuration["Facebook:AppId"] + "|" + _configuration["Facebook:ApiSecret"];
            var verifyTokenResponse = await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={verifyKeys}");

            if (!verifyTokenResponse.IsSuccessStatusCode) return Unauthorized();

            var url = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

            var fbInf = await _httpClient.GetFromJsonAsync<FacebookDto>(url);

            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == fbInf.Email);

            if (user != null) return CreateAppUser(user);

            user = new AppUser
            {
                DisplayName = fbInf.Name,
                Email = fbInf.Email,
                UserName = fbInf.Email,
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        Id = "fb_" + fbInf.Id,
                        Url = fbInf.Picture.Data.Url,
                        IsMain = true
                    }
                }
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem create user");

            return CreateAppUser(user);
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users.Include(r => r.RefreshTokens)
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                return Unauthorized();

            return CreateAppUser(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GetRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private UserDto CreateAppUser(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(x=>x.IsMain)?.Url,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
