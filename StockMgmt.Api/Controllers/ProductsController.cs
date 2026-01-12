using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Product;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Controllers;

[ApiController]
[Route("api/ctrl/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ProductResponseDto>>.Ok(products, "Products retrieved successfully"));
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductResponseDto>.Fail($"Product with ID {id} not found"));
        }
        return Ok(ApiResponse<ProductResponseDto>.Ok(product, "Product retrieved successfully"));
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] ProductCreateDto createDto)
    {
        try
        {
            var product = await _productService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, 
                ApiResponse<ProductResponseDto>.Created(product, "Product created successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<ProductResponseDto>.Fail(ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<ProductResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromBody] ProductUpdateDto updateDto)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<ProductResponseDto>.Ok(product, "Product updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<ProductResponseDto>.Fail(ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<ProductResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete a product (soft delete) - Admin only
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Product with ID {id} not found"));
        }
        return Ok(ApiResponse<object>.Ok(null, "Product deleted successfully"));
    }
}
