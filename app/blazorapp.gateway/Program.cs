using blazorapp.gateway.Connectors;
using blazorapp.gateway.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using microservices.shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JWT Bearer authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5003";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization();

// Register shared services and connectors
builder.Services.RegisterShared();
builder.Services.AddScoped<ICustomerConnector, CustomerConnector>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });
    });
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
