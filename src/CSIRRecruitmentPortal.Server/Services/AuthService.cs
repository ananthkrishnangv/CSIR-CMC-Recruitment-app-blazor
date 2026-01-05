using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSIRRecruitmentPortal.Server.Data;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginApplicantAsync(ApplicantLoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Aadhaar == request.Aadhaar && u.Role == UserRole.APPLICANT);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found. Please check Aadhaar number or register."
            };
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            User = MapToUserDto(user),
            Token = token
        };
    }

    public async Task<AuthResponse> LoginAdminAsync(AdminLoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => 
                (u.Email == request.Username || u.Aadhaar == request.Username) &&
                (u.Role == UserRole.ADMIN || u.Role == UserRole.SUPERVISOR || u.Role == UserRole.DIRECTOR));

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            User = MapToUserDto(user),
            Token = token
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if Aadhaar already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Aadhaar == request.Aadhaar);

        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "This Aadhaar Number is already registered. Please login."
            };
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Mobile = request.Mobile,
            Aadhaar = request.Aadhaar,
            Role = UserRole.APPLICANT,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful! Please login with your Aadhaar number."
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationMustBe32CharsLong!";
        var issuer = _configuration["Jwt:Issuer"] ?? "CSIRRecruitmentPortal";
        var expiryHours = int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("aadhaar", user.Aadhaar ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Mobile = user.Mobile,
        Aadhaar = user.Aadhaar,
        Role = user.Role,
        AvatarUrl = user.AvatarUrl
    };
}
