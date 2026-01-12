using StockMgmt.Core.Entities;

namespace StockMgmt.Core.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ReviewCount { get; set; }
}
