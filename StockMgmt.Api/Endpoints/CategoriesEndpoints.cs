using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Category;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories")
            .WithTags("Categories")
            .WithOpenApi();

        // GET /api/min/categories
        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(categories, "Categories retrieved successfully"));
        })
        .WithName("GetAllCategories")
        .Produces<ApiResponse<IEnumerable<CategoryResponseDto>>>(StatusCodes.Status200OK);

        // GET /api/min/categories/{id}
        group.MapGet("/{id:int}", async (int id, ICategoryService categoryService) =>
        {
            var category = await categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return Results.NotFound(ApiResponse<CategoryResponseDto>.Fail($"Category with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(category, "Category retrieved successfully"));
        })
        .WithName("GetCategoryById")
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status404NotFound);

        // POST /api/min/categories
        group.MapPost("/", async ([FromBody] CategoryCreateDto createDto, ICategoryService categoryService) =>
        {
            try
            {
                var category = await categoryService.CreateAsync(createDto);
                return Results.Created($"/api/min/categories/{category.Id}", ApiResponse<CategoryResponseDto>.Created(category, "Category created successfully"));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("CreateCategory")
        .Accepts<CategoryCreateDto>("application/json")
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status409Conflict);

        // PUT /api/min/categories/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] CategoryUpdateDto updateDto, ICategoryService categoryService) =>
        {
            try
            {
                var category = await categoryService.UpdateAsync(id, updateDto);
                return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(category, "Category updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<CategoryResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("UpdateCategory")
        .Accepts<CategoryUpdateDto>("application/json")
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<CategoryResponseDto>>(StatusCodes.Status409Conflict);

        // DELETE /api/min/categories/{id}
        group.MapDelete("/{id:int}", async (int id, ICategoryService categoryService) =>
        {
            var deleted = await categoryService.DeleteAsync(id);
            if (!deleted)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Category with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<object>.Ok(null, "Category deleted successfully"));
        })
        .WithName("DeleteCategory")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }
}