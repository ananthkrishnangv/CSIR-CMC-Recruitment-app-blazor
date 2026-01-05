using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSIRRecruitmentPortal.Server.Services;
using CSIRRecruitmentPortal.Shared.DTOs;

namespace CSIRRecruitmentPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Applicant login using Aadhaar number
    /// </summary>
    [HttpPost("login/applicant")]
    public async Task<ActionResult<AuthResponse>> LoginApplicant([FromBody] ApplicantLoginRequest request)
    {
        var response = await _authService.LoginApplicantAsync(request);
        if (!response.Success)
        {
            return Unauthorized(response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Admin/Official login using username and password
    /// </summary>
    [HttpPost("login/admin")]
    public async Task<ActionResult<AuthResponse>> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        var response = await _authService.LoginAdminAsync(request);
        if (!response.Success)
        {
            return Unauthorized(response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Register a new applicant
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
