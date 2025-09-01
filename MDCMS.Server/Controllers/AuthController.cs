using MDCMS.Server.Data;
using MDCMS.Server.Models;
using MDCMS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MDCMS.Server.Models.AuthDtos;

namespace MDCMS.Server.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;

        public AuthController(IUserRepository repo, IJwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("AuthController is working.");

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password required.");

            var exists = await _repo.GetByUsernameAsync(req.Username);
            if (exists != null) return Conflict("Username already exists.");

            var user = new User
            {
                Username = req.Username,
                Name = req.Name,
                Email = req.Email,
                Designation = req.Designation,
                PasswordHash = PasswordHasher.Hash(req.Password),
                IsActive = true,
                DateModified = DateTime.UtcNow
            };

            await _repo.CreateAsync(user);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _repo.GetByUsernameAsync(req.Username);
            if (user == null) return Unauthorized();

            if (!PasswordHasher.Verify(req.Password, user.PasswordHash)) return Unauthorized();

            var token = _jwtService.GenerateToken(user, out var expires);
            return Ok(new LoginResponse(token, expires));
        }

        [HttpPut("{username}/update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserRequest req)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null) return NotFound();

            user.Name = req.Name;
            user.Email = req.Email;
            user.Designation = req.Designation;
            user.IsActive = req.IsActive;
            user.AllowedPages = (req.AllowedPages != null ? req.AllowedPages : new List<string>());
            user.DateModified = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
            return Ok(new { message = "Update user successful" });
        }

        [HttpPut("{username}/password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string username, [FromBody] ChangePasswordRequest req)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null) return NotFound();

            if (!PasswordHasher.Verify(req.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password is incorrect.");

            user.PasswordHash = PasswordHasher.Hash(req.NewPassword);
            user.DateModified = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
            return Ok(new { message = "Update password successful"});
        }
    }
}
