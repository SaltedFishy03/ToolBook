using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToolBook.Server.Data;
using ToolBook.Server.DTOs.Auth;
using ToolBook.Server.Models;
using ToolBook.Server.Services;

namespace ToolBook.Server.Tests;

public class AuthServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private JwtTokenService CreateTokenService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes("12345678901234567890123456789012")),
                ["Jwt:Issuer"] = "ToolBook.Tests",
                ["Jwt:Audience"] = "ToolBook.Tests",
                ["Jwt:ExpiresMinutes"] = "60"
            })
            .Build();

        return new JwtTokenService(config);
    }
    
    [Fact] // K6 - Adgangskoden må ikke gemmes som klartekst
    public async Task K6_RegisterAsync_HashesPasswordBeforeSavingUser()
    {
        // Arrange
        var context = CreateContext();
        var hasher = new PasswordHasher<User>();

        var service = new AuthService(
            context,
            hasher,
            CreateTokenService());

        var request = new RegisterRequest
        {
            Name = "Test User",
            Email = "hash@test.dk",
            Password = "Password123!"
        };

        // Act
        await service.RegisterAsync(request);

        var savedUser = await context.Users.SingleAsync(u => u.Email == "hash@test.dk");

        // Assert - den gemte hash må ikke være den oprindelige adgangskode
        Assert.NotEqual(request.Password, savedUser.PasswordHash);

        // Kontroller at den oprindelige adgangskode stadig kan verificeres mod hashen
        var verificationResult = hasher.VerifyHashedPassword(
            savedUser,
            savedUser.PasswordHash,
            request.Password);

        Assert.Equal(PasswordVerificationResult.Success, verificationResult);
    }
}