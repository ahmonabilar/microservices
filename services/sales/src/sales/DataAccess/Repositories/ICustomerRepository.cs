using Sales.DataAccess.Models;

namespace Sales.DataAccess.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}
