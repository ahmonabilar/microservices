SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

CREATE FUNCTION [SalesLT].[ufnGetContactInformation]
(
    @CustomerID INT
)
RETURNS NVARCHAR(400)
WITH SCHEMABINDING
AS
BEGIN
    DECLARE @ContactInfo NVARCHAR(400);

    SELECT @ContactInfo =
        COALESCE(Title + ' ', '') +
        FirstName + ' ' +
        COALESCE(MiddleName + ' ', '') +
        LastName +
        COALESCE(' | ' + EmailAddress, '') +
        COALESCE(' | ' + Phone, '')
    FROM [SalesLT].[Customer]
    WHERE CustomerID = @CustomerID;

    RETURN @ContactInfo;
END;
GO
