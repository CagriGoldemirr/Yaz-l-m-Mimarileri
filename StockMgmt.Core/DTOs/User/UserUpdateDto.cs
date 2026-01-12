using StockMgmt.Core.Entities;

namespace StockMgmt.Core.DTOs.User;

public class UserUpdateDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
}
