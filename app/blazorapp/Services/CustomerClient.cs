using blazorapp.Interfaces;
using microservices.Messages;
using System.Text.Json;

namespace blazorapp.Services;

public interface ICustomerClient
{
    Task<GetAllCustomerSuccess> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerClient : ICustomerClient
{
    private readonly IMessageBusConnector _messageBusConnector;

    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerClient> _logger;
    private readonly IConfiguration _configuration;

    public CustomerClient(IMessageBusConnector messageBusConnector, HttpClient httpClient, ILogger<CustomerClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

public class HelloMessage
{
    public string Text { get; set; }
}

public class HelloMessageSuccess
{
    public string SuccessMessage { get; set; }
}

public class HelloMessageFailure
{
    public string Reason { get; set; }
}
