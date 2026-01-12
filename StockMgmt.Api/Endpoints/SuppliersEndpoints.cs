using Microsoft.AspNetCore.Mvc;
using StockMgmt.Core;
using StockMgmt.Core.DTOs.Supplier;
using StockMgmt.Core.Exceptions;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Api.Endpoints;

public static class SuppliersEndpoints
{
    public static void MapSuppliersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/suppliers")
            .WithTags("Suppliers")
            .WithOpenApi();

        // GET /api/min/suppliers
        group.MapGet("/", async (ISupplierService supplierService) =>
        {
            var suppliers = await supplierService.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<SupplierResponseDto>>.Ok(suppliers, "Suppliers retrieved successfully"));
        })
        .WithName("GetAllSuppliers")
        .Produces<ApiResponse<IEnumerable<SupplierResponseDto>>>(StatusCodes.Status200OK);

        // GET /api/min/suppliers/{id}
        group.MapGet("/{id:int}", async (int id, ISupplierService supplierService) =>
        {
            var supplier = await supplierService.GetByIdAsync(id);
            if (supplier == null)
            {
                return Results.NotFound(ApiResponse<SupplierResponseDto>.Fail($"Supplier with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier retrieved successfully"));
        })
        .WithName("GetSupplierById")
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status404NotFound);

        // POST /api/min/suppliers
        group.MapPost("/", async ([FromBody] SupplierCreateDto createDto, ISupplierService supplierService) =>
        {
            try
            {
                var supplier = await supplierService.CreateAsync(createDto);
                return Results.Created($"/api/min/suppliers/{supplier.Id}", ApiResponse<SupplierResponseDto>.Created(supplier, "Supplier created successfully"));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<SupplierResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("CreateSupplier")
        .Accepts<SupplierCreateDto>("application/json")
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status409Conflict);

        // PUT /api/min/suppliers/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] SupplierUpdateDto updateDto, ISupplierService supplierService) =>
        {
            try
            {
                var supplier = await supplierService.UpdateAsync(id, updateDto);
                return Results.Ok(ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<SupplierResponseDto>.Fail(ex.Message));
            }
        })
        .WithName("UpdateSupplier")
        .Accepts<SupplierUpdateDto>("application/json")
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<SupplierResponseDto>>(StatusCodes.Status404NotFound);

        // DELETE /api/min/suppliers/{id}
        group.MapDelete("/{id:int}", async (int id, ISupplierService supplierService) =>
        {
            var deleted = await supplierService.DeleteAsync(id);
            if (!deleted)
            {
                return Results.NotFound(ApiResponse<object>.Fail($"Supplier with ID {id} not found"));
            }
            return Results.Ok(ApiResponse<object>.Ok(null, "Supplier deleted successfully"));
        })
        .WithName("DeleteSupplier")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }
}