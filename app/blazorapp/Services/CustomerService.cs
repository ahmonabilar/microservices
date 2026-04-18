using System.Net.Http.Headers;
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
    private readonly TokenProvider _tokenProvider;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        HttpClient httpClient,
        TokenProvider tokenProvider,
        ILogger<CustomerService> logger
    )
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<CustomerDto>?> GetAllCustomersAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogInformation("Requesting all customers from gateway");

            var accessToken = await _tokenProvider.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, "Customer");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken
                );
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CustomerDto>>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers from gateway");
            return null;
        }
    }
}
