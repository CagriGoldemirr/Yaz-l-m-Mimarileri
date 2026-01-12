using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Review;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Controllers;

[ApiController]
[Route("api/ctrl/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get all reviews
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReviewResponseDto>>>> GetAll()
    {
        var reviews = await _reviewService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviews, "Reviews retrieved successfully"));
    }

    /// <summary>
    /// Get review by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReviewResponseDto>>> GetById(int id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound(ApiResponse<ReviewResponseDto>.Fail($"Review with ID {id} not found"));
        }
        return Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Review retrieved successfully"));
    }

    /// <summary>
    /// Create a new review
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReviewResponseDto>>> Create([FromBody] ReviewCreateDto createDto)
    {
        try
        {
            var review = await _reviewService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, 
                ApiResponse<ReviewResponseDto>.Created(review, "Review created successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReviewResponseDto>>> Update(int id, [FromBody] ReviewUpdateDto updateDto)
    {
        try
        {
            var review = await _reviewService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Review updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete a review (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _reviewService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Review with ID {id} not found"));
        }
        return Ok(ApiResponse<object>.Ok(null, "Review deleted successfully"));
    }
}
