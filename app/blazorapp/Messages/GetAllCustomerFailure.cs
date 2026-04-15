namespace microservices.Messages
{
    public class GetAllCustomerFailure
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public string? ErrorCode { get; init; }
    }
}
