namespace StockMgmt.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? SKU { get; set; }
    
    // Foreign Keys
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    
    // Navigation Properties
    public virtual Category Category { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
