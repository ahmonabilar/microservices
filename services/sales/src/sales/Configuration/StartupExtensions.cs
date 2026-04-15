using Microsoft.Extensions.DependencyInjection;

namespace Sales.Configuration;

public static class StartupExtensions
{
    /// <summary>
    /// Configures all sales project services including MassTransit consumers.
    /// This should be called during application startup.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    /// <example>
    /// Usage in Program.cs:
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.Services.AddSalesServices();
    /// var app = builder.Build();
    /// </example>
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddSalesServices();
        services.AddSalesMassTransitConsumers();

        return services;
    }
}
