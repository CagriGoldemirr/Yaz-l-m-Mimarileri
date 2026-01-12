namespace StockMgmt.Core.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
