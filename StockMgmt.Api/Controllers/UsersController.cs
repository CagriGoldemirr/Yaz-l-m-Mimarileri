using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.User;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Controllers;

[ApiController]
[Route("api/ctrl/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserResponseDto>>.Ok(users, "Users retrieved successfully"));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserResponseDto>.Fail($"User with ID {id} not found"));
        }
        return Ok(ApiResponse<UserResponseDto>.Ok(user, "User retrieved successfully"));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> Create([FromBody] UserCreateDto createDto)
    {
        try
        {
            var user = await _userService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, 
                ApiResponse<UserResponseDto>.Created(user, "User created successfully"));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<UserResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> Update(int id, [FromBody] UserUpdateDto updateDto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<UserResponseDto>.Ok(user, "User updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<UserResponseDto>.Fail(ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<UserResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"User with ID {id} not found"));
        }
        return Ok(ApiResponse<object>.Ok(null, "User deleted successfully"));
    }
}
