using Sales.DataAccess.Models;
using Sales.DataAccess.Repositories;

namespace Sales.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}
