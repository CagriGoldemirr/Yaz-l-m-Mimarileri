namespace StockMgmt.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
