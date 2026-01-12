using StockMgmt.Core.DTOs.Product;

namespace StockMgmt.Service.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(ProductCreateDto createDto);
    Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto updateDto);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

