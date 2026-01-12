using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.DTOs.Supplier;
using StockMgmt.Core.Entities;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SupplierService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SupplierResponseDto> CreateAsync(SupplierCreateDto createDto)
    {
        var supplier = _mapper.Map<Supplier>(createDto);
        supplier.CreatedAt = DateTime.UtcNow;

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        await _context.Entry(supplier)
            .Collection(s => s.Products)
            .LoadAsync();

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> UpdateAsync(int id, SupplierUpdateDto updateDto)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (supplier == null)
        {
            throw new KeyNotFoundException($"Supplier with ID {id} not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Name))
            supplier.Name = updateDto.Name;
        if (updateDto.ContactEmail != null)
            supplier.ContactEmail = updateDto.ContactEmail;
        if (updateDto.ContactPhone != null)
            supplier.ContactPhone = updateDto.ContactPhone;
        if (updateDto.Address != null)
            supplier.Address = updateDto.Address;

        supplier.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto?> GetByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        return supplier == null ? null : _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync()
    {
        var suppliers = await _context.Suppliers
            .Include(s => s.Products)
            .Where(s => !s.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SupplierResponseDto>>(suppliers);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (supplier == null)
        {
            return false;
        }

        supplier.IsDeleted = true;
        supplier.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}

