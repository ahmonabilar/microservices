using MassTransit;
using Microsoft.Data.SqlClient;
using Sales.DataAccess.Repositories;
using Sales.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection("Server=127.0.0.1,1433;Database=AdventureWorksLT2022;User ID=sa;Password=P@ssw0rd1234;TrustServerCertificate=True;"
        ?? throw new InvalidOperationException("Connection string 'SalesDb' not found.")));

//"Data Source=127.0.0.1,1433;Persist Security Info=True;User ID=sa;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Command Timeout=0"
builder.Services.AddMassTransit(conf =>
{
    conf.SetKebabCaseEndpointNameFormatter();
    conf.SetInMemorySagaRepositoryProvider();

    var assembly = typeof(Program).Assembly;

    conf.AddConsumers(assembly);

    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h => {
            h.Username("user");
            h.Password("password");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Employee service started.";
});

app.Run();