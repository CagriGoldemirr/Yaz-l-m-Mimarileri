namespace StockMgmt.Core.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public enum UserRole
{
    User = 0,
    Admin = 1
}
