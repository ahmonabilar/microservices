using microservices.shared.Interfaces;
using microservices.shared.Messages;

namespace blazorapp.gateway.Connectors;

public interface ICustomerConnector
{
    Task<GetAllCustomerSuccess> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerConnector : ICustomerConnector
{
    private readonly IMessageBusConnector _messageBusConnector;
    private readonly ILogger<CustomerConnector> _logger;

    public CustomerConnector(
        IMessageBusConnector messageBusConnector,
        ILogger<CustomerConnector> logger
    )
    {
        _messageBusConnector =
            messageBusConnector ?? throw new ArgumentNullException(nameof(messageBusConnector));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetAllCustomerSuccess> GetAllCustomersAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogInformation("Requesting all customers from sales service");

            var (success, failure) = await _messageBusConnector.SendMessageSync<
                GetAllCustomer,
                GetAllCustomerSuccess,
                GetAllCustomerFailure
            >(new GetAllCustomer());

            if (failure != null)
            {
                _logger.LogError("Failed to get customers: {ErrorMessage}", failure.ErrorMessage);
                return null;
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers from sales service");
            return null;
        }
    }
}
