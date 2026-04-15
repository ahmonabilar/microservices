using MassTransit;
using microservices.Messages;
using Microsoft.Extensions.Logging;
using Sales.Services;

namespace Sales.Consumers;

public class GetAllCustomerConsumer : IConsumer<GetAllCustomer>
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<GetAllCustomerConsumer> _logger;

    public GetAllCustomerConsumer(ICustomerService customerService, ILogger<GetAllCustomerConsumer> logger)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<GetAllCustomer> context)
    {
        try
        {
            _logger.LogInformation("Processing GetAllCustomer request");

            var customers = await _customerService.GetAllCustomersAsync(context.CancellationToken);

            var customerDtos = customers
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    CompanyName = c.CompanyName,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToList();

            var response = new GetAllCustomerSuccess
            {
                Customers = customerDtos
            };

            await context.RespondAsync(response);

            _logger.LogInformation("GetAllCustomer request processed successfully. Found {CustomerCount} customers", customerDtos.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetAllCustomer request");

            var failureResponse = new GetAllCustomerFailure
            {
                ErrorMessage = "An error occurred while retrieving customers",
                ErrorCode = "INTERNAL_ERROR"
            };

            await context.RespondAsync(failureResponse);
        }
    }
}
