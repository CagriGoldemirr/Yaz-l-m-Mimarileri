using StockMgmt.Core.DTOs.Category;

namespace StockMgmt.Service.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto createDto);
    Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto updateDto);
    Task<CategoryResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

