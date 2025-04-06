using _40PlusRipped.API.Controllers;
using _40PlusRipped.Core.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Policy;

namespace _40PlusRipped.API
{
    [Route("api/auth/google")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GoogleAuthController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("url")]
        public IActionResult GetGoogleAuthUrl()
        {
            // Generate the Google authentication URL
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };

            return Ok(new { url = $"{Request.Scheme}://{Request.Host}/api/auth/google/signin" });
        }

        [HttpGet("signin")]
        public IActionResult SignIn()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
                return Unauthorized();

            var emailClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
            var nameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Name);
            var givenNameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName);
            var surnameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Surname);

            if (emailClaim == null)
                return BadRequest("Email claim not found");

            var email = emailClaim.Value;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Register new user
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = givenNameClaim?.Value ?? nameClaim?.Value.Split(' ')[0] ?? string.Empty,
                    LastName = surnameClaim?.Value ?? (nameClaim?.Value.Split(' ').Length > 1 ? nameClaim.Value.Split(' ')[1] : string.Empty),
                    DateOfBirth = DateTime.UtcNow.AddYears(-40), // Default for 40PlusRipped
                    Gender = "Not specified"
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            // Generate JWT token
            var authController = new AuthController(_userManager, _signInManager, _configuration);
            return await authController.GenerateTokenForUser(user);
        }
    }
}
