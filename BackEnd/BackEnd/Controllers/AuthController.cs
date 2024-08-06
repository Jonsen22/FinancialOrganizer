using BackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicationUser model, bool useCookie = false)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);

                    DateTime now = DateTime.Now;

                    if (useCookie)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = now.AddHours(2),
                            Domain = "localhost"
                        };

                        _httpContextAccessor.HttpContext.Response.Cookies.Append("loginCookie", token, cookieOptions);
                    }

                    return Ok(new { token });
                }
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationUser model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["loginCookie"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = Environment.GetEnvironmentVariable("JWTKey");

                if (string.IsNullOrEmpty(jwtKey))
                    throw new InvalidOperationException("JWTKey environment variable is not set.");

                var key = Encoding.ASCII.GetBytes(jwtKey);

                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);

                var user = GetUserFromToken(refreshToken);
                var newToken = GenerateJwtToken(user);

                DateTime now = DateTime.Now;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = now.AddHours(2)
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("loginCookie", newToken, cookieOptions);

                return Ok(new { token = newToken });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var jwtKey = Environment.GetEnvironmentVariable("JWTKey");
            var issuer = Environment.GetEnvironmentVariable("JWTIssuer");
            var audience = Environment.GetEnvironmentVariable("JWTAudience");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT environment variables are not set.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private IdentityUser GetUserFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                throw new ArgumentException("Invalid JWT token");
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(emailClaim))
            {
                throw new ArgumentException("Email claim not found in JWT token");
            }

            var user = _userManager.FindByEmailAsync(emailClaim).Result;

            return user;
        }
    }

}

