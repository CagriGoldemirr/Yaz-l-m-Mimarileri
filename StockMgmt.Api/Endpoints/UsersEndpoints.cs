using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.User;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi();

        // GET /api/min/users
        group.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<UserResponseDto>>.Ok(users, "Users retrieved successfully"));
        })
        .WithName("GetAllUsers")
        .Produces<ApiResponse<IEnumerable<UserResponseDto>>>(StatusCodes.Status200OK);

        // GET /api/min/users/{id}
        group.MapGet("/{id:int}", async (int id, IUserService userService) =>
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null)
            {
                return Results.NotFound(ApiResponse<UserResponseDto>.Fail($"User with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<UserResponseDto>.Ok(user, "User retrieved successfully"));
        })
        .WithName("GetUserById")
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status404NotFound);

        // POST /api/min/users
        group.MapPost("/", async ([FromBody] UserCreateDto createDto, IUserService userService) =>
        {
            try
            {
                var user = await userService.CreateAsync(createDto);
                return Results.Created($"/api/min/users/{user.Id}", ApiResponse<UserResponseDto>.Created(user, "User created successfully"));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<UserResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("CreateUser")
        .Accepts<UserCreateDto>("application/json")
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status409Conflict);

        // PUT /api/min/users/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] UserUpdateDto updateDto, IUserService userService) =>
        {
            try
            {
                var user = await userService.UpdateAsync(id, updateDto);
                return Results.Ok(ApiResponse<UserResponseDto>.Ok(user, "User updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<UserResponseDto>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<UserResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("UpdateUser")
        .Accepts<UserUpdateDto>("application/json")
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status409Conflict);

        // DELETE /api/min/users/{id}
        group.MapDelete("/{id:int}", async (int id, IUserService userService) =>
        {
            var deleted = await userService.DeleteAsync(id);
            if (!deleted)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"User with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<object>.Ok(null, "User deleted successfully"));
        })
        .WithName("DeleteUser")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }
}