using Microsoft.Extensions.DependencyInjection;
using Sales.DataAccess.Repositories;
using Sales.Services;

namespace Sales.Configuration;

public static class ServiceConfiguration
{
    /// <summary>
    /// Adds sales services and repositories to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSalesServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Register services
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
