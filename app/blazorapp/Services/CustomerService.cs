using System.Net.Http.Json;
using microservices.shared.Dtos;

namespace blazorapp.Services;

public interface ICustomerService
{
    Task<List<CustomerDto>?> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(HttpClient httpClient, ILogger<CustomerService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<CustomerDto>?> GetAllCustomersAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogInformation("Requesting all customers from gateway");
            return await _httpClient.GetFromJsonAsync<List<CustomerDto>>(
                "Customer",
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers from gateway");
            return null;
        }
    }
}
