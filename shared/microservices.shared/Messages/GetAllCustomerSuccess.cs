using microservices.shared.Dtos;

namespace microservices.shared.Messages;

public record GetAllCustomerSuccess
{
    public List<CustomerDto> Customers { get; init; } = new();
}
