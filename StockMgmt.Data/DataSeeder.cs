using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.Entities;

namespace StockMgmt.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if database is empty
        if (await context.Users.AnyAsync())
        {
            return; // Database already has data
        }

        // Seed Users (Admin and User)
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!");

        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = adminPasswordHash,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        var regularUser = new User
        {
            Username = "user",
            PasswordHash = userPasswordHash,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, regularUser);
        await context.SaveChangesAsync();

        // Seed Categories
        var categories = new List<Category>
        {
            new Category
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Clothing",
                Description = "Apparel and fashion items",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Books",
                Description = "Books and literature",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Home & Garden",
                Description = "Home improvement and garden supplies",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Sports & Outdoors",
                Description = "Sports equipment and outdoor gear",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // Seed Suppliers
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Name = "TechCorp Inc.",
                ContactEmail = "contact@techcorp.com",
                ContactPhone = "+1-555-0101",
                Address = "123 Tech Street, Silicon Valley, CA 94000",
                CreatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "Fashion World Ltd.",
                ContactEmail = "info@fashionworld.com",
                ContactPhone = "+1-555-0202",
                Address = "456 Fashion Avenue, New York, NY 10001",
                CreatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "BookStore Publishers",
                ContactEmail = "sales@bookstore.com",
                ContactPhone = "+1-555-0303",
                Address = "789 Library Lane, Boston, MA 02101",
                CreatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "Home Essentials Co.",
                ContactEmail = "support@homeessentials.com",
                ContactPhone = "+1-555-0404",
                Address = "321 Home Road, Chicago, IL 60601",
                CreatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "Sports Gear Pro",
                ContactEmail = "orders@sportsgear.com",
                ContactPhone = "+1-555-0505",
                Address = "654 Sports Boulevard, Denver, CO 80201",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();

        // Seed Products
        var products = new List<Product>
        {
            new Product
            {
                Name = "Smartphone Pro Max",
                Description = "Latest generation smartphone with advanced features",
                Price = 999.99m,
                StockQuantity = 50,
                SKU = "ELEC-001",
                CategoryId = categories[0].Id,
                SupplierId = suppliers[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Wireless Headphones",
                Description = "Premium noise-cancelling wireless headphones",
                Price = 299.99m,
                StockQuantity = 100,
                SKU = "ELEC-002",
                CategoryId = categories[0].Id,
                SupplierId = suppliers[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Laptop Ultra",
                Description = "High-performance laptop for professionals",
                Price = 1499.99m,
                StockQuantity = 30,
                SKU = "ELEC-003",
                CategoryId = categories[0].Id,
                SupplierId = suppliers[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Cotton T-Shirt",
                Description = "Comfortable 100% cotton t-shirt",
                Price = 19.99m,
                StockQuantity = 200,
                SKU = "CLOTH-001",
                CategoryId = categories[1].Id,
                SupplierId = suppliers[1].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Denim Jeans",
                Description = "Classic fit denim jeans",
                Price = 49.99m,
                StockQuantity = 150,
                SKU = "CLOTH-002",
                CategoryId = categories[1].Id,
                SupplierId = suppliers[1].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Programming Guide",
                Description = "Comprehensive guide to modern programming",
                Price = 39.99m,
                StockQuantity = 75,
                SKU = "BOOK-001",
                CategoryId = categories[2].Id,
                SupplierId = suppliers[2].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mystery Novel",
                Description = "Bestselling mystery thriller",
                Price = 14.99m,
                StockQuantity = 120,
                SKU = "BOOK-002",
                CategoryId = categories[2].Id,
                SupplierId = suppliers[2].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Garden Tool Set",
                Description = "Complete set of gardening tools",
                Price = 79.99m,
                StockQuantity = 60,
                SKU = "HOME-001",
                CategoryId = categories[3].Id,
                SupplierId = suppliers[3].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Yoga Mat",
                Description = "Premium non-slip yoga mat",
                Price = 29.99m,
                StockQuantity = 90,
                SKU = "SPORT-001",
                CategoryId = categories[4].Id,
                SupplierId = suppliers[4].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Running Shoes",
                Description = "Professional running shoes with cushioning",
                Price = 89.99m,
                StockQuantity = 80,
                SKU = "SPORT-002",
                CategoryId = categories[4].Id,
                SupplierId = suppliers[4].Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Seed Reviews
        var reviews = new List<Review>
        {
            new Review
            {
                Title = "Excellent smartphone!",
                Content = "This is the best smartphone I've ever used. Great camera and battery life.",
                Rating = 5,
                UserId = regularUser.Id,
                ProductId = products[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Good but expensive",
                Content = "The phone is great but the price is quite high.",
                Rating = 4,
                UserId = adminUser.Id,
                ProductId = products[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Amazing sound quality",
                Content = "Best headphones I've ever owned. Noise cancellation works perfectly.",
                Rating = 5,
                UserId = regularUser.Id,
                ProductId = products[1].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Comfortable and stylish",
                Content = "Love the fit and style of this t-shirt. Very comfortable.",
                Rating = 5,
                UserId = regularUser.Id,
                ProductId = products[3].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Great programming book",
                Content = "Very comprehensive and well-written. Highly recommend for beginners.",
                Rating = 5,
                UserId = adminUser.Id,
                ProductId = products[5].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Thrilling read",
                Content = "Couldn't put it down! Great mystery novel with unexpected twists.",
                Rating = 5,
                UserId = regularUser.Id,
                ProductId = products[6].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Perfect for gardening",
                Content = "All the tools I need in one set. Good quality and durable.",
                Rating = 4,
                UserId = adminUser.Id,
                ProductId = products[7].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Best yoga mat",
                Content = "Non-slip surface works great. Very comfortable for all poses.",
                Rating = 5,
                UserId = regularUser.Id,
                ProductId = products[8].Id,
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Title = "Comfortable running shoes",
                Content = "Great cushioning and support. Perfect for long runs.",
                Rating = 5,
                UserId = adminUser.Id,
                ProductId = products[9].Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();
    }
}


