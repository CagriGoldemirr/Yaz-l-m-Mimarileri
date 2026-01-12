using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockMgmt.Core.DTOs.Auth;
using StockMgmt.Core.Entities;
using StockMgmt.Core.Exceptions;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username && !u.IsDeleted);

        if (existingUser != null)
        {
            throw new ConflictException($"Username '{registerDto.Username}' already exists.");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, BCrypt.Net.BCrypt.GenerateSalt());

        // Create user
        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            Role = registerDto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationInMinutes"))
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username && !u.IsDeleted);

        if (user == null)
        {
            throw new NotFoundException("Invalid username or password.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new NotFoundException("Invalid username or password.");
        }

        // Generate token
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationInMinutes"))
        };
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "StockMgmtApi";
        var audience = _configuration["JwtSettings:Audience"] ?? "StockMgmtApiUsers";
        var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes", 60);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

