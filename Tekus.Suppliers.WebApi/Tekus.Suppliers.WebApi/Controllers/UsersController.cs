using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tekus.Suppliers.WebApi.Application.DTOs.UserDTOs;

namespace Tekus.Suppliers.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;        
        private readonly IOutputCacheStore _outputCacheStore;        
        private const string cacheTag = "users";

        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, IOutputCacheStore outputCacheStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;            
            _outputCacheStore = outputCacheStore;
        }
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userCredetialsDTO"></param>
        /// <returns>Return JWT</returns>
        /// <response code="200">Returns a JWT token</response>
        /// <response code="400">If the registration fails</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDTO>> Register(UserCredetialsDTO userCredetialsDTO)
        {
            var user = new IdentityUser
            {
                UserName = userCredetialsDTO.Email,
                Email = userCredetialsDTO.Email
            };

            var result = await _userManager.CreateAsync(user, userCredetialsDTO.Password);

            if (result.Succeeded)
            {
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                return await BuildToken(user);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="userCredetialsDTO"></param>
        /// <returns>Returns a JWT token</returns>
        /// <response code="200">Returns a JWT token</response>
        /// <response code="400">If the login fails</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login(UserCredetialsDTO userCredetialsDTO)
        {
            var user = await _userManager.FindByEmailAsync(userCredetialsDTO.Email);

            if (user is null)
            {
                var errors = BuildIncorrectLoginErrorMessage();
                return BadRequest(errors);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user,
                userCredetialsDTO.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }
            else
            {
                var errors = BuildIncorrectLoginErrorMessage();
                return BadRequest(errors);

            }
        }
        /// <summary>
        /// Creates a new admin claim for a user
        /// </summary>
        /// <param name="editClaimDTO"></param>
        /// <returns></returns>
        /// <response code="204">Returns no content</response>
        /// <response code="404">If the user is not found</response>
        [HttpPost("makeadmin")]
        public async Task<IActionResult> MakeAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await _userManager.FindByEmailAsync(editClaimDTO.Email);
            if (user is null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user, new Claim("isadmin", "true"));
            return NoContent();
        }
        /// <summary>
        /// Removes the admin claim from a user.
        /// </summary>
        /// <param name="editClaimDTO"></param>
        /// <returns></returns>
        /// <response code="204">Returns no content</response>
        /// <response code="404">If the user is not found</response>
        [HttpPost("removeadmin")]
        public async Task<IActionResult> RemoveAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await _userManager.FindByEmailAsync(editClaimDTO.Email);

            if (user is null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim("isadmin", "true"));
            return NoContent();
        }

        private IEnumerable<IdentityError> BuildIncorrectLoginErrorMessage()
        {
            var identityError = new IdentityError() { Description = "Incorrect login" };
            var errors = new List<IdentityError>();
            errors.Add(identityError);
            return errors;
        }

        private async Task<AuthenticationResponseDTO> BuildToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("email", user.Email!),
                new Claim("role", "admin")
            };

            var claimsDB = await _userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtkey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO
            {
                Token = token,
                Expiration = expiration
            };
        }
    }
}
