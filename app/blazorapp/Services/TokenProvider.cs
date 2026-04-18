using System.Net.Http.Json;
using System.Text.Json;

namespace blazorapp.Services;

public class TokenProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenProvider> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset _expiresAt;

    public TokenProvider(IHttpClientFactory httpClientFactory, ILogger<TokenProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void Initialize(string? accessToken, string? refreshToken, DateTimeOffset expiresAt)
    {
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _expiresAt = expiresAt;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (!IsExpired)
            return _accessToken;

        if (string.IsNullOrEmpty(_refreshToken))
            return _accessToken;

        await _semaphore.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (!IsExpired)
                return _accessToken;

            await RefreshAsync();
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private bool IsExpired =>
        string.IsNullOrEmpty(_accessToken)
        || DateTimeOffset.UtcNow >= _expiresAt.AddSeconds(-30);

    private async Task RefreshAsync()
    {
        _logger.LogInformation("Access token expired, refreshing using refresh token");

        using var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(
            "https://localhost:5003/connect/token",
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = "web_client",
                    ["refresh_token"] = _refreshToken!,
                }
            )
        );

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            _accessToken = json.GetProperty("access_token").GetString();
            if (json.TryGetProperty("refresh_token", out var rt))
                _refreshToken = rt.GetString();
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(
                json.GetProperty("expires_in").GetInt32()
            );
            _logger.LogInformation("Access token refreshed successfully");
        }
        else
        {
            _logger.LogWarning(
                "Failed to refresh access token. Status: {StatusCode}",
                response.StatusCode
            );
        }
    }
}
