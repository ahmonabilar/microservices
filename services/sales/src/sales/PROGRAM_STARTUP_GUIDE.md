# Program.cs and Startup.cs Usage Guide

This guide explains the generated `Program.cs` and `Startup.cs` files for the Sales project and how to use them.

## Overview

The Sales project is now configured as an ASP.NET Core Web API with .NET 10. It includes:
- MassTransit consumer registration
- Dependency injection configuration
- Database connection management
- Swagger/OpenAPI documentation
- Logging configuration

## File Descriptions

### Program.cs (Recommended - .NET 10 Minimal Hosting)

The `Program.cs` file uses the modern .NET minimal hosting model introduced in .NET 6. This is the **recommended approach** for new projects.

**Key features:**
- Concise and readable
- Direct service registration
- Automatic dependency injection setup
- Built-in middleware pipeline configuration

**What it does:**
1. Creates a `WebApplication` builder
2. Adds ASP.NET Core services (Controllers, Swagger)
3. Configures the database connection from `appsettings.json`
4. Registers all Sales services and MassTransit consumers via `AddSalesModule()`
5. Configures logging for console and debug output
6. Builds the application
7. Configures middleware (Swagger for development, HTTPS, authorization)
8. Maps controllers and starts the application

**Usage:**
The `Program.cs` is automatically used when you run the application:
```bash
dotnet run
```

### Startup.cs (Alternative - Traditional Pattern)

The `Startup.cs` file provides the traditional ASP.NET Core startup pattern used in older projects. Use this if your organization prefers the classic approach.

**Key features:**
- Separate configuration and setup methods
- Explicit `ConfigureServices` and `Configure` methods
- Familiar pattern from ASP.NET Framework

**To use Startup.cs instead of Program.cs:**

1. Delete or rename `Program.cs`
2. Modify your application entry point or configuration

**Note:** In .NET 10, the minimal hosting model (`Program.cs`) is preferred as it's simpler and more maintainable.

## Configuration Files

### appsettings.json

Contains application configuration including:
- Logging levels
- Database connection string (SalesDb)
- Allowed hosts

**Connection String:**
```json
"ConnectionStrings": {
  "SalesDb": "Server=localhost;Database=SalesLT;Integrated Security=true;Encrypt=false;"
}
```

Update this to match your SQL Server connection details.

### appsettings.Development.json

Development-specific overrides:
- Increased logging verbosity
- Debug logging enabled

## Running the Application

### Prerequisites
- .NET 10 SDK installed
- SQL Server with SalesLT database
- Appropriate connection string in `appsettings.json`

### Starting the Application

```bash
# Restore NuGet packages
dotnet restore

# Run the application
dotnet run

# Run in watch mode (for development)
dotnet watch run

# Build only
dotnet build

# Run with specific environment
dotnet run --environment Production
```

### Accessing the Application

Once running:
- **API Base URL:** `https://localhost:5001` (or `http://localhost:5000`)
- **Swagger UI:** `https://localhost:5001/swagger/ui` (in development only)

## Service Registration Flow

When the application starts, the following occurs:

1. **Program.cs / Startup.cs** executes
2. **`AddSalesModule()`** is called, which:
   - Calls `AddSalesServices()` to register:
     - `ICustomerService` → `CustomerService` (Scoped)
     - `ICustomerRepository` → `CustomerRepository` (Scoped)
   - Calls `AddSalesMassTransitConsumers()` to register:
     - MassTransit bus configuration
     - `GetAllCustomerConsumer` (and any future consumers)
3. **Database connection** is configured as a scoped service
4. **Logging** is configured
5. **Middleware pipeline** is set up
6. Application is ready to handle requests

## MassTransit Consumers

The application automatically registers the following consumers:

### GetAllCustomerConsumer
- **Accepts:** `GetAllCustomer` message
- **Responds with:** 
  - `GetAllCustomerSuccess` (on success)
  - `GetAllCustomerFailure` (on error)
- **Uses:** `ICustomerService` to fetch customers from the database

### Adding New Consumers

To add a new consumer:

1. Create your consumer class implementing `IConsumer<TMessage>`
2. Register it in `Configuration/MassTransitConfiguration.cs`:
   ```csharp
   busConfigurator.AddConsumer<YourNewConsumer>();
   ```
3. It will be automatically loaded on next application start

## Dependency Injection

All components use constructor injection:

```csharp
public class YourClass
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<YourClass> _logger;

    public YourClass(ICustomerService customerService, ILogger<YourClass> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }
}
```

**Configured Lifetimes:**
- **Singleton:** MassTransit bus
- **Scoped:** Services, Repositories, Database connections (per HTTP request)
- **Transient:** Controllers

## Troubleshooting

### Application fails to start
- Check SQL Server is running
- Verify connection string in `appsettings.json`
- Ensure database exists and contains required tables

### Cannot access Swagger UI
- Swagger is only available in Development environment
- Check `ASPNETCORE_ENVIRONMENT=Development`

### MassTransit consumers not loaded
- Verify consumer is registered in `MassTransitConfiguration.cs`
- Check logs for initialization errors

### Database connection issues
- Verify `SalesDb` connection string
- Test connection with SQL Server Management Studio
- Check SQL Server security/authentication settings

## Environment-Specific Configuration

### Development
```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run
```
- Swagger enabled
- Debug logging
- Detailed error messages

### Production
```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run
```
- Swagger disabled
- Information logging level
- Error handling only

Update `appsettings.Production.json` as needed.
