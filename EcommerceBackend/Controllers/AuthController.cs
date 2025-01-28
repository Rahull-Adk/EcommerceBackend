using EcommerceBackend.Helper;
using EcommerceBackend.Models;
using ECommerceBackend.Data;
using EcommerceBackend.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
namespace ECommerceBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email is already in use.");
            }
           
            var passwordHasher = new PasswordHasher<UserModel>();
            var user = new UserModel
            {
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            Console.WriteLine("Admin email: " + _config["AdminCredentials:Email"]);
            Console.WriteLine("Admin password: " + _config["AdminCredentials:Password"]);
            if (request.Email == _config["AdminCredentials:Email"] && request.Password == _config["AdminCredentials:Password"])
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "Customer";
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var passwordHasher = new PasswordHasher<UserModel>();
            var password = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (PasswordVerificationResult.Success != password)
            {
            }
            Console.WriteLine(_config["JwtSettings:SecretKey"]);
            Console.WriteLine(_config["JwtSettings:Issuer"]);
            Console.WriteLine(_config["JwtSettings:Audience"]);
            var token = JwtTokenHelper.GenerateJwtToken(user.Username, user.Role,
                _config["JwtSettings:Issuer"]!,
                _config["JwtSettings:Audience"]!,
                _config["JwtSettings:SecretKey"]!);

            return Ok(new { Token = token });
        }
        [Authorize]
        [HttpGet("/me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == User.Identity!.Name);
            return Ok(user);
        }


    }
}
