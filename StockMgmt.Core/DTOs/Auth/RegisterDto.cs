using StockMgmt.Core.Entities;

namespace StockMgmt.Core.DTOs.Auth;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
}


