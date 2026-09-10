using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Auth;
using ToolBook.Server.Models;
using ToolBook.Server.Services;
using LoginRequest = ToolBook.Server.DTOs.Auth.LoginRequest;
using RegisterRequest = ToolBook.Server.DTOs.Auth.RegisterRequest;

namespace ToolBook.Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(ApplicationDbContext context, IPasswordHasher<User> hasher, JwtTokenService tokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest registerRequest)
        {
            //check if user already exists
            var userExists = await context.Users.AnyAsync(u => u.Email == registerRequest.Email);
            if (userExists)
            {
                return Conflict("En bruger med denne email findes allerede");
            }
            
            var registeredUser = new User
            {
                Name = registerRequest.Name,
                Email = registerRequest.Email,
            };

            //hash password
            var hashedPassword = hasher.HashPassword(registeredUser, registerRequest.Password);

            registeredUser.PasswordHash = hashedPassword;
            context.Users.Add(registeredUser);
            await context.SaveChangesAsync();

            var token = tokenService.CreateToken(registeredUser);
            
            var response = new AuthResponse
            {
                Id = registeredUser.Id,
                Name = registeredUser.Name,
                Email = registeredUser.Email,
                Role = registeredUser.Role.ToString(),
                Token = token
            };
            return response;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest loginRequest)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return Unauthorized("Forkert email eller adgangskode");
            }

            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Forkert email eller adgangskode");
            }

            var token = tokenService.CreateToken(user);
            var response = new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            };
            return response;
        }
    }
}
