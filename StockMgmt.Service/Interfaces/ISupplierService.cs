using StockMgmt.Core.DTOs.Supplier;

namespace StockMgmt.Service.Interfaces;

public interface ISupplierService
{
    Task<SupplierResponseDto> CreateAsync(SupplierCreateDto createDto);
    Task<SupplierResponseDto> UpdateAsync(int id, SupplierUpdateDto updateDto);
    Task<SupplierResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

