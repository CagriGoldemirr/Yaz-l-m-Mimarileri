using StockMgmt.Core.Entities;

namespace StockMgmt.Core.DTOs.User;

public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
