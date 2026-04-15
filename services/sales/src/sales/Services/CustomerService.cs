using Sales.DataAccess.Models;
using Sales.DataAccess.Repositories;

namespace Sales.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetAllCustomersAsync(cancellationToken);
    }
}
