using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Review;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Endpoints;

public static class ReviewsEndpoints
{
    public static void MapReviewsEndpoints(this IEndpointRouteBuilder app)
    {
        // Nested route: /products/{productId}/reviews
        var nestedGroup = app.MapGroup("/products/{productId:int}/reviews")
            .WithTags("Reviews")
            .WithOpenApi();

        // GET /api/min/products/{productId}/reviews
        nestedGroup.MapGet("/", async (int productId, IReviewService reviewService, IProductService productService) =>
        {
            // Verify product exists
            var product = await productService.GetByIdAsync(productId);
            if (product == null)
            {
                return Results.NotFound(ApiResponse<IEnumerable<ReviewResponseDto>>.Fail($"Product with ID {productId} not found"));
            }

            // Get all reviews and filter by product name
            var allReviews = await reviewService.GetAllAsync();
            var productReviews = allReviews.Where(r => r.ProductName == product.Name).ToList();
            
            return Results.Ok(ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(productReviews, $"Reviews for product {productId} retrieved successfully"));
        })
        .WithName("GetReviewsByProductId")
        .Produces<ApiResponse<IEnumerable<ReviewResponseDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<IEnumerable<ReviewResponseDto>>>(StatusCodes.Status404NotFound);

        // GET /api/min/products/{productId}/reviews/{id}
        nestedGroup.MapGet("/{id:int}", async (int productId, int id, IReviewService reviewService, IProductService productService) =>
        {
            // Verify product exists
            var product = await productService.GetByIdAsync(productId);
            if (product == null)
            {
                return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail($"Product with ID {productId} not found"));
            }

            var review = await reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail($"Review with ID {id} not found"));
            }

            // Verify review belongs to product
            if (review.ProductName != product.Name)
            {
                return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail($"Review {id} does not belong to product {productId}"));
            }

            return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Review retrieved successfully"));
        })
        .WithName("GetReviewById")
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status400BadRequest);

        // POST /api/min/products/{productId}/reviews
        nestedGroup.MapPost("/", async (int productId, [FromBody] ReviewCreateDto createDto, IReviewService reviewService, IProductService productService) =>
        {
            try
            {
                // Ensure productId in DTO matches route parameter
                createDto.ProductId = productId;

                var review = await reviewService.CreateAsync(createDto);
                return Results.Created($"/api/min/products/{productId}/reviews/{review.Id}", 
                    ApiResponse<ReviewResponseDto>.Created(review, "Review created successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("CreateReview")
        .Accepts<ReviewCreateDto>("application/json")
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status400BadRequest);

        // PUT /api/min/products/{productId}/reviews/{id}
        nestedGroup.MapPut("/{id:int}", async (int productId, int id, [FromBody] ReviewUpdateDto updateDto, IReviewService reviewService, IProductService productService) =>
        {
            try
            {
                // Verify product exists
                var product = await productService.GetByIdAsync(productId);
                if (product == null)
                {
                    return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail($"Product with ID {productId} not found"));
                }

                var review = await reviewService.UpdateAsync(id, updateDto);
                
                // Verify review belongs to product
                if (review.ProductName != product.Name)
                {
                    return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail($"Review {id} does not belong to product {productId}"));
                }

                return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Review updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("UpdateReview")
        .Accepts<ReviewUpdateDto>("application/json")
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status400BadRequest);

        // DELETE /api/min/products/{productId}/reviews/{id}
        nestedGroup.MapDelete("/{id:int}", async (int productId, int id, IReviewService reviewService, IProductService productService) =>
        {
            // Verify product exists
            var product = await productService.GetByIdAsync(productId);
            if (product == null)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Product with ID {productId} not found"));
            }

            var review = await reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Review with ID {id} not found"));
            }

            // Verify review belongs to product
            if (review.ProductName != product.Name)
            {
                return Results.BadRequest(ApiResponse<object>.Fail($"Review {id} does not belong to product {productId}"));
            }

            var deleted = await reviewService.DeleteAsync(id);
            if (!deleted)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Review with ID {id} not found"));
            }

            return Results.Ok(ApiResponse<object>.Ok(null, "Review deleted successfully"));
        })
        .WithName("DeleteReview")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);

        // Also add standalone routes for reviews (optional, for flexibility)
        var standaloneGroup = app.MapGroup("/reviews")
            .WithTags("Reviews")
            .WithOpenApi();

        // GET /api/min/reviews
        standaloneGroup.MapGet("/", async (IReviewService reviewService) =>
        {
            var reviews = await reviewService.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviews, "Reviews retrieved successfully"));
        })
        .WithName("GetAllReviews")
        .Produces<ApiResponse<IEnumerable<ReviewResponseDto>>>(StatusCodes.Status200OK);

        // GET /api/min/reviews/{id}
        standaloneGroup.MapGet("/{id:int}", async (int id, IReviewService reviewService) =>
        {
            var review = await reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail($"Review with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Review retrieved successfully"));
        })
        .WithName("GetReviewByIdStandalone")
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound);
    }
}