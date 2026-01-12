using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.DTOs.Product;
using StockMgmt.Core.Entities;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto createDto)
    {
        // Check SKU uniqueness if provided
        if (!string.IsNullOrWhiteSpace(createDto.SKU))
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == createDto.SKU && !p.IsDeleted);
            
            if (existingProduct != null)
            {
                throw new InvalidOperationException($"Product with SKU '{createDto.SKU}' already exists.");
            }
        }

        // Verify Category exists
        var category = await _context.Categories.FindAsync(createDto.CategoryId);
        if (category == null || category.IsDeleted)
        {
            throw new KeyNotFoundException($"Category with ID {createDto.CategoryId} not found.");
        }

        // Verify Supplier exists
        var supplier = await _context.Suppliers.FindAsync(createDto.SupplierId);
        if (supplier == null || supplier.IsDeleted)
        {
            throw new KeyNotFoundException($"Supplier with ID {createDto.SupplierId} not found.");
        }

        var product = _mapper.Map<Product>(createDto);
        product.CreatedAt = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Load related entities for response
        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();
        await _context.Entry(product)
            .Reference(p => p.Supplier)
            .LoadAsync();
        await _context.Entry(product)
            .Collection(p => p.Reviews)
            .LoadAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto updateDto)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        // Check SKU uniqueness if updated
        if (!string.IsNullOrWhiteSpace(updateDto.SKU) && updateDto.SKU != product.SKU)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == updateDto.SKU && p.Id != id && !p.IsDeleted);
            
            if (existingProduct != null)
            {
                throw new InvalidOperationException($"Product with SKU '{updateDto.SKU}' already exists.");
            }
            product.SKU = updateDto.SKU;
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(updateDto.Name))
            product.Name = updateDto.Name;
        if (updateDto.Description != null)
            product.Description = updateDto.Description;
        if (updateDto.Price.HasValue)
            product.Price = updateDto.Price.Value;
        if (updateDto.StockQuantity.HasValue)
            product.StockQuantity = updateDto.StockQuantity.Value;
        
        if (updateDto.CategoryId.HasValue)
        {
            var category = await _context.Categories.FindAsync(updateDto.CategoryId.Value);
            if (category == null || category.IsDeleted)
            {
                throw new KeyNotFoundException($"Category with ID {updateDto.CategoryId.Value} not found.");
            }
            product.CategoryId = updateDto.CategoryId.Value;
        }

        if (updateDto.SupplierId.HasValue)
        {
            var supplier = await _context.Suppliers.FindAsync(updateDto.SupplierId.Value);
            if (supplier == null || supplier.IsDeleted)
            {
                throw new KeyNotFoundException($"Supplier with ID {updateDto.SupplierId.Value} not found.");
            }
            product.SupplierId = updateDto.SupplierId.Value;
        }

        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload related entities
        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();
        await _context.Entry(product)
            .Reference(p => p.Supplier)
            .LoadAsync();
        await _context.Entry(product)
            .Collection(p => p.Reviews)
            .LoadAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        return product == null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Reviews)
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            return false;
        }

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}

