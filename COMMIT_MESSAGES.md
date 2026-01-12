# Commit Messages - Stock Management API

## 1. Initial Project Setup
```
feat: initialize solution and project structure

- Create StockMgmt.sln solution
- Add StockMgmt.Api (ASP.NET Core Web API)
- Add StockMgmt.Core (class library)
- Add StockMgmt.Data (class library)
- Add StockMgmt.Service (class library)
- Configure project references (Api->Core,Service,Data; Service->Core,Data; Data->Core)
- Create folder structure (Controllers, Endpoints, Middleware, Mapping, Logging, Validators)
```

## 2. NuGet Packages Installation
```
chore: install required NuGet packages

- Add PostgreSQL packages (Npgsql, Npgsql.EntityFrameworkCore.PostgreSQL)
- Add EF Core packages (Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design)
- Add Swagger packages (Swashbuckle.AspNetCore)
- Add JWT packages (Microsoft.AspNetCore.Authentication.JwtBearer)
- Add logging packages (Serilog.AspNetCore)
- Add AutoMapper packages (AutoMapper, AutoMapper.Extensions.Microsoft.DependencyInjection)
- Add validation packages (FluentValidation)
```

## 3. Domain Entities
```
feat: add domain entities with EF Core configuration

- Create BaseEntity with common properties (Id, CreatedAt, UpdatedAt, IsDeleted)
- Create User entity with Role enum (Admin/User)
- Create Category, Supplier, Product, Review entities
- Configure EF Core navigation properties and relationships
- Add unique indexes (Username, Category.Name, Product.SKU)
- Configure global query filters for soft delete
- Set up cascade delete behavior
```

## 4. DTOs and API Response Model
```
feat: add DTOs and standardize API response format

- Create CreateDto, UpdateDto, ResponseDto for each entity
- Implement ApiResponse<T> model with static factory methods
- Add non-generic ApiResponse class
- Ensure ResponseDto includes only necessary fields for relationships
- Enforce API layer to return only DTOs, not raw entities
```

## 5. Service Layer Implementation
```
feat: implement service layer with CRUD operations

- Create service interfaces (IProductService, ICategoryService, ISupplierService, IUserService, IReviewService)
- Implement service classes with business logic
- Add uniqueness validation (Username, SKU, Category.Name)
- Implement password hashing with BCrypt
- Add soft delete functionality
- Configure AutoMapper for entity-DTO mapping
- Register services in DI container
```

## 6. Global Exception Handling
```
feat: add global exception handling middleware

- Create ExceptionHandlingMiddleware
- Map exceptions to HTTP status codes (NotFoundException->404, ConflictException->409, ValidationException->400)
- Return standardized ApiResponse format for all exceptions
- Add logging for exceptions
- Configure middleware in request pipeline
```

## 7. Minimal API Endpoints
```
feat: implement Minimal API endpoint groups

- Create UsersEndpoints, CategoriesEndpoints, SuppliersEndpoints, ProductsEndpoints, ReviewsEndpoints
- Implement CRUD operations for all entities
- Add nested routes for reviews (/products/{productId}/reviews)
- Return ApiResponse format for all endpoints
- Register endpoint groups under /api/min prefix
- Add proper HTTP status codes (200/201/404/409)
```

## 8. MVC Controllers
```
feat: add MVC controllers with CRUD operations

- Create UsersController, CategoriesController, SuppliersController, ProductsController, ReviewsController
- Configure route prefix /api/ctrl/[controller]
- Implement CRUD operations using service layer
- Return ApiResponse format for all actions
- Add Swagger documentation attributes
- Register controllers in Program.cs
```

## 9. Swagger/OpenAPI Configuration
```
feat: configure Swagger/OpenAPI with JWT Bearer support

- Add SwaggerGen configuration
- Configure JWT Bearer security scheme
- Add Authorize button in Swagger UI
- Enable Swagger UI in development environment
- Configure XML comments for documentation
- Ensure both Minimal APIs and Controllers appear in Swagger
```

## 10. JWT Authentication
```
feat: implement JWT authentication and authorization

- Add BCrypt.Net-Next for password hashing
- Create AuthService with Register and Login methods
- Create AuthController with /api/auth/register and /api/auth/login endpoints
- Configure JWT Bearer authentication in Program.cs
- Add authorization policies (AdminOnly, UserOrAdmin)
- Protect Product.Delete endpoint with Admin role requirement
- Add JWT settings to appsettings.json
```

## 11. Database Seeding
```
feat: add database seed data

- Create DataSeeder class with SeedAsync method
- Seed Admin and User accounts with hashed passwords
- Seed Categories, Suppliers, Products, and Reviews
- Implement automatic seeding on application startup
- Check if database is empty before seeding
- Add BCrypt.Net-Next to Data project
```

## 12. Status Code Standardization
```
fix: standardize HTTP status codes across all endpoints

- Update Create endpoints to return 201 Created
- Change Delete endpoints from 204 to 200 OK with ApiResponse
- Ensure all NotFound scenarios return 404
- Ensure all Conflict scenarios return 409
- Add missing ConflictException handling in Suppliers endpoints
- Verify all endpoints return ApiResponse format
- Update ProducesResponseType attributes for Swagger documentation
```


