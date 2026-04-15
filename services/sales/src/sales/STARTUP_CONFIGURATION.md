# Sales Project Startup Configuration

This document describes how to configure the Sales project and ensure all MassTransit consumers are loaded during application startup.

## Configuration Overview

The Sales project provides extension methods for dependency injection that make it easy to register all services, repositories, and MassTransit consumers.

## Usage

### Option 1: Use the Complete Module (Recommended)

In your application's `Program.cs`, add the following:

```csharp
using Sales.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add all Sales services and consumers
builder.Services.AddSalesModule();

var app = builder.Build();
app.Run();
```

This will:
- Register the `ICustomerService` and `CustomerService`
- Register the `ICustomerRepository` and `CustomerRepository`
- Register and configure MassTransit with all consumers (including `GetAllCustomerConsumer`)
- Configure the MassTransit bus for message handling

### Option 2: Register Services and MassTransit Separately

If you prefer more control:

```csharp
using Sales.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register only sales services
builder.Services.AddSalesServices();

// Register and configure MassTransit with sales consumers
builder.Services.AddSalesMassTransitConsumers();

var app = builder.Build();
app.Run();
```

## Available Consumers

The following consumers are automatically loaded during startup:

- **`GetAllCustomerConsumer`** - Handles `GetAllCustomer` messages and responds with either:
  - `GetAllCustomerSuccess` (containing a list of customers)
  - `GetAllCustomerFailure` (if an error occurs)

## Adding New Consumers

When you add a new consumer to the Sales project:

1. Create your consumer class that implements `IConsumer<TMessage>`
2. Register it in `MassTransitConfiguration.cs`:

```csharp
busConfigurator.AddConsumer<YourNewConsumer>();
```

3. The consumer will be automatically loaded during the next application startup

## Dependency Injection

All components use constructor injection and are configured with the appropriate lifetimes:

- **Repositories** - `Scoped` (per request)
- **Services** - `Scoped` (per request)
- **MassTransit** - Managed by MassTransit container

## Database Connection

The `CustomerRepository` requires an `IDbConnection` to be registered. Make sure to configure your database connection in your host application:

```csharp
builder.Services.AddScoped<IDbConnection>(sp => 
    new SqlConnection(builder.Configuration.GetConnectionString("SalesDb")));
```

## Stored Procedures

- **`sp_GetAllCustomers`** - Called by `GetAllCustomerConsumer` to fetch all customers from the database
