using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Sales.Consumers;

namespace Sales.Configuration;

public static class MassTransitConfiguration
{
    /// <summary>
    /// Adds MassTransit with all consumers from the Sales project to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSalesMassTransitConsumers(this IServiceCollection services)
    {
        services.AddMassTransit(busConfigurator =>
        {
            // Register all consumers from the Sales project
            busConfigurator.AddConsumer<GetAllCustomerConsumer>();

            // Configure the bus with common settings
            busConfigurator.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
