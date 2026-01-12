namespace StockMgmt.Core.Entities;

public class Review : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    
    // Foreign Keys
    public int UserId { get; set; }
    public int ProductId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
