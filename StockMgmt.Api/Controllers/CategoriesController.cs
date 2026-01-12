using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Category;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Controllers;

[ApiController]
[Route("api/ctrl/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponseDto>>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(categories, "Categories retrieved successfully"));
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(ApiResponse<CategoryResponseDto>.Fail($"Category with ID {id} not found"));
        }
        return Ok(ApiResponse<CategoryResponseDto>.Ok(category, "Category retrieved successfully"));
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> Create([FromBody] CategoryCreateDto createDto)
    {
        try
        {
            var category = await _categoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, 
                ApiResponse<CategoryResponseDto>.Created(category, "Category created successfully"));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> Update(int id, [FromBody] CategoryUpdateDto updateDto)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<CategoryResponseDto>.Ok(category, "Category updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Category with ID {id} not found"));
        }
        return Ok(ApiResponse<object>.Ok(null, "Category deleted successfully"));
    }
}
