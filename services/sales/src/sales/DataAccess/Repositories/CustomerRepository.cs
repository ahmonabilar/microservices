using Dapper;
using Sales.DataAccess.Models;
using System.Data;

namespace Sales.DataAccess.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnection _dbConnection;

    public CustomerRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "[SalesLT].[sp_GetAllCustomers]";

        var result = await _dbConnection.QueryAsync<Customer>(
            sql,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 30);

        return result;
    }
}
