using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.DTOs.Category;
using StockMgmt.Core.Entities;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto createDto)
    {
        // Check Name uniqueness
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == createDto.Name && !c.IsDeleted);
        
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category with name '{createDto.Name}' already exists.");
        }

        var category = _mapper.Map<Category>(createDto);
        category.CreatedAt = DateTime.UtcNow;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _context.Entry(category)
            .Collection(c => c.Products)
            .LoadAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto updateDto)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        // Check Name uniqueness if updated
        if (!string.IsNullOrWhiteSpace(updateDto.Name) && updateDto.Name != category.Name)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == updateDto.Name && c.Id != id && !c.IsDeleted);
            
            if (existingCategory != null)
            {
                throw new InvalidOperationException($"Category with name '{updateDto.Name}' already exists.");
            }
            category.Name = updateDto.Name;
        }

        if (updateDto.Description != null)
            category.Description = updateDto.Description;

        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (category == null)
        {
            return false;
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}

