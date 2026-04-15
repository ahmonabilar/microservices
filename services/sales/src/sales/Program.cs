using System.Data;
using MassTransit;
using Microsoft.Data.SqlClient;
using Sales.DataAccess.Repositories;
using Sales.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(
    config.GetValue<string>("ConnectionStrings:SalesDb")
        ?? throw new InvalidOperationException("Connection string 'SalesDb' not found.")
));

builder.Services.AddMassTransit(conf =>
{
    conf.SetKebabCaseEndpointNameFormatter();
    conf.SetInMemorySagaRepositoryProvider();

    var assembly = typeof(Program).Assembly;

    conf.AddConsumers(assembly);

    conf.UsingRabbitMq(
        (ctx, cfg) =>
        {
            cfg.Host(
                config.GetValue<string>("RabbitMQ:Host"),
                h =>
                {
                    h.Username(
                        config.GetValue<string>("RabbitMQ:Username")
                            ?? throw new InvalidOperationException(
                                "Configuration 'RabbitMQ:Username' not found."
                            )
                    );
                    h.Password(
                        config.GetValue<string>("RabbitMQ:Password")
                            ?? throw new InvalidOperationException(
                                "Configuration 'RabbitMQ:Password' not found."
                            )
                    );
                }
            );

            cfg.ConfigureEndpoints(ctx);
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet(
    "/",
    () =>
    {
        return "Sales service started.";
    }
);

app.Run();
