namespace StockMgmt.Core.DTOs.Review;

public class ReviewCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
}

