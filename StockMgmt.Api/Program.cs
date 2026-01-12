using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StockMgmt.Api.Endpoints;
using StockMgmt.Api.Middleware;
using StockMgmt.Data;
using StockMgmt.Service.Implementations;
using StockMgmt.Service.Interfaces;
using StockMgmt.Service.Mappings;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stock Management API",
        Version = "v1",
        Description = "Stock Management API with Minimal API and MVC Controllers"
    });

    // Add JWT Bearer security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=StockMgmtDb.db"));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "StockMgmtApi",
        ValidAudience = jwtSettings["Audience"] ?? "StockMgmtApiUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

var app = builder.Build();

// Initialize database and seed if empty
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Check if migrations exist, if not use EnsureCreated for SQLite
        try
        {
            var allMigrations = context.Database.GetMigrations();
            
            if (allMigrations.Any())
            {
                // Migrations exist, apply them
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                // No migrations exist, use EnsureCreated for SQLite
                logger.LogInformation("No migrations found, ensuring database is created...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database ensured created successfully.");
            }
        }
        catch (Exception migrationCheckEx)
        {
            // If we can't check migrations (e.g., database schema issue), use EnsureCreated
            logger.LogWarning(migrationCheckEx, "Could not check migrations, using EnsureCreated...");
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Database ensured created successfully.");
        }
        
        // Seed database if empty
        await DataSeeder.SeedAsync(context);
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing or seeding the database.");
        // Don't crash the app - allow it to continue
    }
}

// Configure the HTTP request pipeline.
// Add exception handling middleware at the beginning
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Management API v1");
        c.RoutePrefix = "swagger"; // Swagger UI at /swagger
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map endpoint groups with /api/min prefix
var apiGroup = app.MapGroup("/api/min").WithTags("Stock Management API");

// Register all endpoint groups
apiGroup.MapUsersEndpoints();
apiGroup.MapCategoriesEndpoints();
apiGroup.MapSuppliersEndpoints();
apiGroup.MapProductsEndpoints();
apiGroup.MapReviewsEndpoints();

app.Run();
