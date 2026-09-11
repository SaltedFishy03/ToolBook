using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Auth;
using ToolBook.Server.Models;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Services;

public class AuthService(
    ApplicationDbContext context,
    IPasswordHasher<User> hasher,
    JwtTokenService tokenService) : IAuthService
{
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var userExists = await context.Users.AnyAsync(u => u.Email == request.Email);

        if (userExists)
        {
            return null;
        }

        var registeredUser = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        // Hash password
        registeredUser.PasswordHash =
            hasher.HashPassword(registeredUser, request.Password);

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

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return null;
        }

        var verificationResult = hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
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