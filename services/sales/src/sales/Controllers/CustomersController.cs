using microservices.Messages;
using Microsoft.AspNetCore.Mvc;
using Sales.Services;

namespace Sales.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<GetAllCustomerSuccess>> GetAllCustomers(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("HTTP request received for GetAllCustomers");

            var customers = await _customerService.GetAllCustomersAsync(cancellationToken);

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

            _logger.LogInformation("HTTP GetAllCustomers request processed successfully. Found {CustomerCount} customers", customerDtos.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing HTTP GetAllCustomers request");
            return StatusCode(500, new { message = "An error occurred while retrieving customers" });
        }
    }
}
