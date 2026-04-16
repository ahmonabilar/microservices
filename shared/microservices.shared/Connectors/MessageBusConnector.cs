using MassTransit;
using microservices.shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace microservices.shared.Connectors;

public class MessageBusConnector : IMessageBusConnector
{
    private readonly IServiceProvider _serviceProvider;

    public MessageBusConnector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<(TSuccess success, TFailure failure)> SendMessageSync<
        TRequest,
        TSuccess,
        TFailure
    >(TRequest request)
        where TRequest : class
        where TSuccess : class
        where TFailure : class
    {
        var clientFactory = _serviceProvider.GetRequiredService<IClientFactory>();

        var client = clientFactory.CreateRequestClient<TRequest>();

        var (success, failure) = await client.GetResponse<TSuccess, TFailure>(request);

        TSuccess successResponse = null;
        TFailure failureResponse = null;

        if (success?.IsCompletedSuccessfully ?? false)
        {
            var response = await success;
            successResponse = response.Message;
        }

        if (failure?.IsCompletedSuccessfully ?? false)
        {
            var response = await failure;
            failureResponse = response.Message;
        }

        return (success: successResponse, failure: failureResponse);
    }
}
