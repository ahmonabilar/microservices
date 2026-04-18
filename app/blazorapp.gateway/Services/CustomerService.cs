using blazorapp.gateway.Connectors;
using microservices.shared.Dtos;

namespace blazorapp.gateway.Services;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerConnector _customerConnector;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerConnector customerConnector, ILogger<CustomerService> logger)
    {
        _customerConnector = customerConnector ?? throw new ArgumentNullException(nameof(customerConnector));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        var result = await _customerConnector.GetAllCustomersAsync(cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Failed to retrieve customers");
            return new List<CustomerDto>();
        }

        return result.Customers;
    }
}
