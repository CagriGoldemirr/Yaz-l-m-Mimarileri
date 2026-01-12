using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Product;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products")
            .WithOpenApi();

        // GET /api/min/products
        group.MapGet("/", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<ProductResponseDto>>.Ok(products, "Products retrieved successfully"));
        })
        .WithName("GetAllProducts")
        .Produces<ApiResponse<IEnumerable<ProductResponseDto>>>(StatusCodes.Status200OK);

        // GET /api/min/products/{id}
        group.MapGet("/{id:int}", async (int id, IProductService productService) =>
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                return Results.NotFound(ApiResponse<ProductResponseDto>.Fail($"Product with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<ProductResponseDto>.Ok(product, "Product retrieved successfully"));
        })
        .WithName("GetProductById")
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status404NotFound);

        // POST /api/min/products
        group.MapPost("/", async ([FromBody] ProductCreateDto createDto, IProductService productService) =>
        {
            try
            {
                var product = await productService.CreateAsync(createDto);
                return Results.Created($"/api/min/products/{product.Id}", ApiResponse<ProductResponseDto>.Created(product, "Product created successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<ProductResponseDto>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<ProductResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("CreateProduct")
        .Accepts<ProductCreateDto>("application/json")
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status409Conflict);

        // PUT /api/min/products/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] ProductUpdateDto updateDto, IProductService productService) =>
        {
            try
            {
                var product = await productService.UpdateAsync(id, updateDto);
                return Results.Ok(ApiResponse<ProductResponseDto>.Ok(product, "Product updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<ProductResponseDto>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<ProductResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("UpdateProduct")
        .Accepts<ProductUpdateDto>("application/json")
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status409Conflict);

        // DELETE /api/min/products/{id}
        group.MapDelete("/{id:int}", async (int id, IProductService productService) =>
        {
            var deleted = await productService.DeleteAsync(id);
            if (!deleted)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Product with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<object>.Ok(null, "Product deleted successfully"));
        })
        .WithName("DeleteProduct")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }
}