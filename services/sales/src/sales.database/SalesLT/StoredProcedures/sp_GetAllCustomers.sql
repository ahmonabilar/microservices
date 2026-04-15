-- Stored Procedure: [SalesLT].[sp_GetAllCustomers]
-- Description: Retrieves all customers from the Customer table
CREATE PROCEDURE [SalesLT].[sp_GetAllCustomers]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [CustomerID] AS CustomerId,
        [FirstName],
        [LastName],
        [CompanyName],
        [EmailAddress] AS Email,
        [Phone]
    FROM [SalesLT].[Customer]
    ORDER BY [CustomerID];
END;
