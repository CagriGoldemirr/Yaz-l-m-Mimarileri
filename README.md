# Stock Management API

A comprehensive .NET 9.0 RESTful API for managing stock inventory with support for products, categories, suppliers, users, and reviews. Built with clean architecture principles, JWT authentication, and PostgreSQL database.

## 📋 Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [API Response Format](#api-response-format)
- [Installation & Setup](#installation--setup)
- [JWT Authentication](#jwt-authentication)
- [Logging](#logging)
- [Database Seeding](#database-seeding)

## ✨ Features

- **Clean Architecture**: Separation of concerns with Core, Data, Service, and API layers
- **JWT Authentication**: Secure token-based authentication with role-based authorization
- **CRUD Operations**: Full CRUD support for Users, Categories, Suppliers, Products, and Reviews
- **Soft Delete**: Soft delete functionality for all entities
- **Dual API Style**: Both Minimal API endpoints and MVC Controllers
- **Swagger/OpenAPI**: Interactive API documentation with JWT Bearer support
- **AutoMapper**: Automatic DTO mapping
- **Global Exception Handling**: Centralized exception handling with standardized responses
- **Database Seeding**: Automatic seed data on first run
- **BCrypt Password Hashing**: Secure password storage

## 🏗️ Architecture

### Architecture Diagram

```mermaid
graph TB
    subgraph "API Layer - StockMgmt.Api"
        A[Controllers] --> B[Endpoints]
        A --> C[Middleware]
        B --> D[Services]
        C --> D
    end
    
    subgraph "Service Layer - StockMgmt.Service"
        D --> E[Service Interfaces]
        D --> F[Service Implementations]
        F --> G[AutoMapper]
        F --> H[Data Layer]
    end
    
    subgraph "Data Layer - StockMgmt.Data"
        H --> I[ApplicationDbContext]
        I --> J[PostgreSQL Database]
    end
    
    subgraph "Core Layer - StockMgmt.Core"
        E --> K[Entities]
        E --> L[DTOs]
        E --> M[Exceptions]
        E --> N[ApiResponse]
        K --> O[BaseEntity]
    end
    
    style A fill:#e1f5ff
    style B fill:#e1f5ff
    style D fill:#fff4e1
    style H fill:#e8f5e9
    style K fill:#f3e5f5
```

### Layer Responsibilities

- **StockMgmt.Api**: HTTP endpoints, middleware, request/response handling
- **StockMgmt.Service**: Business logic, validation, DTO mapping
- **StockMgmt.Data**: Database context, EF Core configuration, migrations
- **StockMgmt.Core**: Domain entities, DTOs, exceptions, shared models

## 🛠️ Technology Stack

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **PostgreSQL** (via Npgsql)
- **JWT Bearer Authentication**
- **Swagger/OpenAPI**
- **AutoMapper**
- **BCrypt.Net-Next**
- **FluentValidation**

## 📁 Project Structure

```
StockMgmt.sln
├── StockMgmt.Api/
│   ├── Controllers/          # MVC Controllers
│   ├── Endpoints/            # Minimal API Endpoints
│   ├── Middleware/           # Exception Handling Middleware
│   └── Program.cs            # Application entry point
├── StockMgmt.Core/
│   ├── Entities/             # Domain entities
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Exceptions/           # Custom exceptions
│   └── ApiResponse.cs        # Standard API response model
├── StockMgmt.Data/
│   ├── ApplicationDbContext.cs
│   └── DataSeeder.cs         # Database seeding
└── StockMgmt.Service/
    ├── Interfaces/           # Service interfaces
    ├── Implementations/      # Service implementations
    └── Mappings/             # AutoMapper profiles
```

## 🔌 API Endpoints

### Authentication Endpoints

#### MVC Controller: `/api/auth`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and get JWT token | No |

### Minimal API Endpoints: `/api/min`

#### Users Endpoints: `/api/min/users`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/users` | Get all users | No |
| GET | `/api/min/users/{id}` | Get user by ID | No |
| POST | `/api/min/users` | Create a new user | No |
| PUT | `/api/min/users/{id}` | Update a user | No |
| DELETE | `/api/min/users/{id}` | Delete a user (soft delete) | No |

#### Categories Endpoints: `/api/min/categories`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/categories` | Get all categories | No |
| GET | `/api/min/categories/{id}` | Get category by ID | No |
| POST | `/api/min/categories` | Create a new category | No |
| PUT | `/api/min/categories/{id}` | Update a category | No |
| DELETE | `/api/min/categories/{id}` | Delete a category (soft delete) | No |

#### Suppliers Endpoints: `/api/min/suppliers`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/suppliers` | Get all suppliers | No |
| GET | `/api/min/suppliers/{id}` | Get supplier by ID | No |
| POST | `/api/min/suppliers` | Create a new supplier | No |
| PUT | `/api/min/suppliers/{id}` | Update a supplier | No |
| DELETE | `/api/min/suppliers/{id}` | Delete a supplier (soft delete) | No |

#### Products Endpoints: `/api/min/products`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/products` | Get all products | No |
| GET | `/api/min/products/{id}` | Get product by ID | No |
| POST | `/api/min/products` | Create a new product | No |
| PUT | `/api/min/products/{id}` | Update a product | No |
| DELETE | `/api/min/products/{id}` | Delete a product (soft delete) | No |

#### Reviews Endpoints: `/api/min/products/{productId}/reviews`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/products/{productId}/reviews` | Get all reviews for a product | No |
| GET | `/api/min/products/{productId}/reviews/{id}` | Get review by ID | No |
| POST | `/api/min/products/{productId}/reviews` | Create a new review | No |
| PUT | `/api/min/products/{productId}/reviews/{id}` | Update a review | No |
| DELETE | `/api/min/products/{productId}/reviews/{id}` | Delete a review (soft delete) | No |

#### Standalone Reviews Endpoints: `/api/min/reviews`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/min/reviews` | Get all reviews | No |
| GET | `/api/min/reviews/{id}` | Get review by ID | No |

### MVC Controllers: `/api/ctrl`

#### Users Controller: `/api/ctrl/users`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ctrl/users` | Get all users | No |
| GET | `/api/ctrl/users/{id}` | Get user by ID | No |
| POST | `/api/ctrl/users` | Create a new user | No |
| PUT | `/api/ctrl/users/{id}` | Update a user | No |
| DELETE | `/api/ctrl/users/{id}` | Delete a user (soft delete) | No |

#### Categories Controller: `/api/ctrl/categories`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ctrl/categories` | Get all categories | No |
| GET | `/api/ctrl/categories/{id}` | Get category by ID | No |
| POST | `/api/ctrl/categories` | Create a new category | No |
| PUT | `/api/ctrl/categories/{id}` | Update a category | No |
| DELETE | `/api/ctrl/categories/{id}` | Delete a category (soft delete) | No |

#### Suppliers Controller: `/api/ctrl/suppliers`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ctrl/suppliers` | Get all suppliers | No |
| GET | `/api/ctrl/suppliers/{id}` | Get supplier by ID | No |
| POST | `/api/ctrl/suppliers` | Create a new supplier | No |
| PUT | `/api/ctrl/suppliers/{id}` | Update a supplier | No |
| DELETE | `/api/ctrl/suppliers/{id}` | Delete a supplier (soft delete) | No |

#### Products Controller: `/api/ctrl/products`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ctrl/products` | Get all products | No |
| GET | `/api/ctrl/products/{id}` | Get product by ID | No |
| POST | `/api/ctrl/products` | Create a new product | No |
| PUT | `/api/ctrl/products/{id}` | Update a product | No |
| DELETE | `/api/ctrl/products/{id}` | Delete a product (soft delete) | **Admin Only** |

#### Reviews Controller: `/api/ctrl/reviews`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/ctrl/reviews` | Get all reviews | No |
| GET | `/api/ctrl/reviews/{id}` | Get review by ID | No |
| POST | `/api/ctrl/reviews` | Create a new review | No |
| PUT | `/api/ctrl/reviews/{id}` | Update a review | No |
| DELETE | `/api/ctrl/reviews/{id}` | Delete a review (soft delete) | No |

## 📦 API Response Format

All endpoints return responses in a standardized `ApiResponse<T>` format:

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "name": "Product Name",
    "price": 99.99
  }
}
```

### Created Response (201 Created)

```json
{
  "success": true,
  "message": "Resource created successfully",
  "data": {
    "id": 1,
    "name": "New Product",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

### Delete Response (200 OK)

```json
{
  "success": true,
  "message": "Product deleted successfully",
  "data": null
}
```

### Error Response (404 Not Found)

```json
{
  "success": false,
  "message": "Product with ID 999 not found",
  "data": null
}
```

### Conflict Response (409 Conflict)

```json
{
  "success": false,
  "message": "Username 'admin' already exists.",
  "data": null
}
```

### Validation Error Response (400 Bad Request)

```json
{
  "success": false,
  "message": "Validation failed: Name: Name is required; Price: Price must be greater than 0",
  "data": null
}
```

### Unauthorized Response (401 Unauthorized)

```json
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}
```

### Internal Server Error (500)

```json
{
  "success": false,
  "message": "An error occurred while processing your request.",
  "data": null
}
```

## 🚀 Installation & Setup

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 12+ (or Docker)
- Visual Studio 2022 / VS Code / Rider

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd 1101
```

### Step 2: Configure Database Connection

Update `appsettings.json` with your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=StockMgmtDb;Username=postgres;Password=your_password"
  }
}
```

### Step 3: Create Database and Run Migrations

```bash
# Navigate to the API project
cd StockMgmt.Api

# Create initial migration
dotnet ef migrations add InitialCreate --project ../StockMgmt.Data --startup-project .

# Apply migrations to database
dotnet ef database update --project ../StockMgmt.Data --startup-project .
```

### Step 4: Run the Application

```bash
# From the solution root
dotnet run --project StockMgmt.Api/StockMgmt.Api.csproj
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`

### Step 5: Verify Database Seeding

On first run, the application will automatically seed the database with:
- 2 users (admin/user)
- 5 categories
- 5 suppliers
- 10 products
- 9 reviews

**Default Credentials:**
- **Admin**: `admin` / `Admin123!`
- **User**: `user` / `User123!`

## 🔐 JWT Authentication

### Register a New User

**Request:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "newuser",
  "password": "SecurePass123!",
  "role": "User"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "newuser",
    "role": "User",
    "expiresAt": "2024-01-15T11:30:00Z"
  }
}
```

### Login

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "role": "Admin",
    "expiresAt": "2024-01-15T11:30:00Z"
  }
}
```

### Using JWT Token

#### Option 1: Swagger UI

1. Click the **"Authorize"** button in Swagger UI
2. Enter: `Bearer <your-token>`
3. Click **"Authorize"**
4. All protected endpoints will now use the token

#### Option 2: HTTP Header

```http
GET /api/ctrl/products/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Option 3: cURL

```bash
curl -X GET "https://localhost:5001/api/ctrl/products/1" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Protected Endpoints

- **Admin Only**: `DELETE /api/ctrl/products/{id}` requires Admin role
- Other endpoints are currently public (can be protected as needed)

### JWT Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "StockMgmtApi",
    "Audience": "StockMgmtApiUsers",
    "ExpirationInMinutes": 60
  }
}
```

**⚠️ Important**: Change the `SecretKey` in production!

## 📝 Logging

The application uses **Serilog** for structured logging. Logging is configured in `Program.cs` and includes:

### Log Levels

- **Information**: General application flow
- **Warning**: Non-critical issues (e.g., NotFound, Conflict exceptions)
- **Error**: Unhandled exceptions and critical errors

### Logged Events

1. **Exception Handling**: All exceptions are logged with context
   - NotFoundException → Warning
   - ConflictException → Warning
   - ValidationException → Warning
   - Unhandled exceptions → Error

2. **Database Operations**: EF Core logs database queries (in Development)

3. **Authentication**: Login attempts and failures

### Log Output

Logs are written to:
- **Console**: Development environment
- **File**: Production environment (configurable)

### Example Log Entry

```
[2024-01-15 10:30:00] [WRN] Conflict: Username 'admin' already exists.
[2024-01-15 10:31:00] [ERR] Unhandled Exception: NullReferenceException at ...
```

## 🌱 Database Seeding

The application automatically seeds the database on first run if it's empty. The seed data includes:

### Users
- **Admin**: `admin` / `Admin123!` (Role: Admin)
- **User**: `user` / `User123!` (Role: User)

### Categories (5)
- Electronics
- Clothing
- Books
- Home & Garden
- Sports & Outdoors

### Suppliers (5)
- TechCorp Inc.
- Fashion World Ltd.
- BookStore Publishers
- Home Essentials Co.
- Sports Gear Pro

### Products (10)
- Various products across all categories with realistic data

### Reviews (9)
- Reviews for different products by different users

**Note**: Seeding only runs if the database is empty (no users exist).

## 📊 Status Codes

| Status Code | Usage |
|-------------|-------|
| 200 OK | Successful GET, PUT, DELETE operations |
| 201 Created | Successful POST (Create) operations |
| 400 Bad Request | Validation errors |
| 401 Unauthorized | Missing or invalid JWT token |
| 403 Forbidden | Insufficient permissions (e.g., non-Admin trying to delete product) |
| 404 Not Found | Resource not found |
| 409 Conflict | Duplicate resource (e.g., username already exists) |
| 500 Internal Server Error | Unhandled server errors |

## 🔧 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add XML comments for public APIs
- Keep methods focused and small

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📞 Support

For issues and questions, please open an issue in the repository.


