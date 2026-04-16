using microservices.shared.Interfaces;
using microservices.shared.Messages;

namespace blazorapp.Services;

public interface ICustomerClient
{
    Task<GetAllCustomerSuccess> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerClient : ICustomerClient
{
    private readonly IMessageBusConnector _messageBusConnector;

    private readonly ILogger<CustomerClient> _logger;

    public CustomerClient(IMessageBusConnector messageBusConnector, ILogger<CustomerClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageBusConnector = messageBusConnector ?? throw new ArgumentNullException(nameof(messageBusConnector));
    }

    public async Task<GetAllCustomerSuccess> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Requesting all customers from sales service");


            var (success, failure) = await _messageBusConnector
                .SendMessageSync<GetAllCustomer, GetAllCustomerSuccess, GetAllCustomerFailure>(
                    new GetAllCustomer()
                    {
                    }
                );

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers from sales service");
            return null;
        }
    }
}
