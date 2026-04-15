namespace microservices.Messages;

public record GetAllCustomerFailure
{
    public string ErrorMessage { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
}
