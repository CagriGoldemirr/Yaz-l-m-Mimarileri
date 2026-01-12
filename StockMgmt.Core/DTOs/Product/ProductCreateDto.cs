namespace StockMgmt.Core.DTOs.Product;

public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? SKU { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}

