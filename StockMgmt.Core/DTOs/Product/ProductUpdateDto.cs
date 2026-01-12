namespace StockMgmt.Core.DTOs.Product;

public class ProductUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? SKU { get; set; }
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }
}

