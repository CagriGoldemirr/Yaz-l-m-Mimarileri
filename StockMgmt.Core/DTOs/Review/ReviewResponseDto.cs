namespace StockMgmt.Core.DTOs.Review;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Related entity information (only necessary fields)
    public string Username { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}

