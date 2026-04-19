using blazorapp.gateway.Connectors;
using blazorapp.gateway.Services;
using MassTransit;
using microservices.shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JWT Bearer authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Identity:Authority"] ?? "https://microservices.identity.local";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
            };
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });
builder.Services.AddAuthorization();

// Register shared services and connectors
builder.Services.RegisterShared();
builder.Services.AddScoped<ICustomerConnector, CustomerConnector>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq(
        (ctx, cfg) =>
        {
            var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "user";
            var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "password";
            cfg.Host(
                rabbitHost,
                "/",
                h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                }
            );
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
