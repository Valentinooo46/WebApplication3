using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication3.DTOs;
using WebApplication3.Models;
using WebApplication3.Services;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleSettings _google;
        private readonly JwtSettings _jwt;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IOptions<GoogleSettings> googleOptions,
            IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _google = googleOptions.Value;
            _jwt = jwtOptions.Value;
        }

        // ------------------- LOCAL REGISTER -------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null) return BadRequest(new { error = "Email already registered" });

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IconUrl = dto.IconUrl
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Registered" });
        }

        // ------------------- LOCAL LOGIN -------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();

            var passOk = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!passOk.Succeeded) return Unauthorized();

            var token = GenerateJwt(user);
            return Ok(new
            {
                token,
                user = new { user.Email, user.FirstName, user.LastName, user.IconUrl }
            });
        }

        // ------------------- PROFILE -------------------
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return Ok(new { user.Email, user.FirstName, user.LastName, user.IconUrl });
        }

        // ------------------- GOOGLE LOGIN (PKCE) -------------------
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthRequest req)
        {
            if (string.IsNullOrEmpty(req.Code))
                return BadRequest(new { error = "code_required" });

            // 1) Обмін коду на токени
            var client = _httpClientFactory.CreateClient();
            var tokenRequest = new Dictionary<string, string>
            {
                ["code"] = req.Code,
                ["client_id"] = _google.ClientId,
                ["client_secret"] = _google.ClientSecret,
                ["redirect_uri"] = string.IsNullOrEmpty(req.RedirectUri) ? _google.RedirectUri : req.RedirectUri,
                ["grant_type"] = "authorization_code",
                ["code_verifier"] = req.CodeVerifier
            };

            using var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error = "token_exchange_failed", details = errorText });
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.IdToken))
                return BadRequest(new { error = "invalid_token_response" });

            // 2) Валідація id_token
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _google.ClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "id_token_validation_failed", details = ex.Message });
            }

            // 3) Знайти або створити користувача в AspNetUsers
            var existingUser = await _userManager.FindByEmailAsync(payload.Email);
            if (existingUser == null)
            {
                existingUser = new AppUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? "",
                    IconUrl = payload.Picture ?? ""
                };
                var createRes = await _userManager.CreateAsync(existingUser);
                if (!createRes.Succeeded) return BadRequest(createRes.Errors);
            }

            // 4) Генеруємо JWT для цього користувача
            var appToken = GenerateJwt(existingUser);

            return Ok(new
            {
                token = appToken,
                google = new
                {
                    tokenResponse.AccessToken,
                    tokenResponse.RefreshToken,
                    tokenResponse.ExpiresIn,
                    tokenResponse.Scope,
                    tokenResponse.TokenType,
                    id_token = tokenResponse.IdToken
                },
                user = new
                {
                    existingUser.Email,
                    existingUser.FirstName,
                    existingUser.LastName,
                    existingUser.IconUrl
                }
            });
        }

        // ------------------- JWT GENERATION -------------------
        private string GenerateJwt(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("firstName", user.FirstName ?? ""),
                new Claim("lastName", user.LastName ?? ""),
                new Claim("iconUrl", user.IconUrl ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}