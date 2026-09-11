using Microsoft.AspNetCore.Mvc;
using ToolBook.Server.DTOs.Auth;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request);

        if (response == null)
        {
            return Conflict("En bruger med denne email findes allerede");
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized("Forkert email eller adgangskode");
        }

        return Ok(response);
    }
}