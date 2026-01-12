using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Supplier;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Controllers;

[ApiController]
[Route("api/ctrl/[controller]")]
[Produces("application/json")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    /// <summary>
    /// Get all suppliers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SupplierResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SupplierResponseDto>>>> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SupplierResponseDto>>.Ok(suppliers, "Suppliers retrieved successfully"));
    }

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> GetById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound(ApiResponse<SupplierResponseDto>.Fail($"Supplier with ID {id} not found"));
        }
        return Ok(ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier retrieved successfully"));
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Create([FromBody] SupplierCreateDto createDto)
    {
        try
        {
            var supplier = await _supplierService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, 
                ApiResponse<SupplierResponseDto>.Created(supplier, "Supplier created successfully"));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<SupplierResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing supplier
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Update(int id, [FromBody] SupplierUpdateDto updateDto)
    {
        try
        {
            var supplier = await _supplierService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<SupplierResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete a supplier (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _supplierService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Supplier with ID {id} not found"));
        }
        return Ok(ApiResponse<object>.Ok(null, "Supplier deleted successfully"));
    }
}
