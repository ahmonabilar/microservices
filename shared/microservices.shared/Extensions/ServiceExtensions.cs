using microservices.shared.Connectors;
using microservices.shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace microservices.shared.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterShared(this IServiceCollection services)
        {
            services.AddScoped<IMessageBusConnector, MessageBusConnector>();
        }
    }
}
