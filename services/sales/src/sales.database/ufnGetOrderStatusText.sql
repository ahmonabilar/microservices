SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

CREATE FUNCTION [SalesLT].[ufnGetOrderStatusText]
(
    @Status TINYINT
)
RETURNS NVARCHAR(50)
WITH SCHEMABINDING
AS
BEGIN
    RETURN (
        SELECT CASE @Status
            WHEN 1 THEN N'In Process'
            WHEN 2 THEN N'Shipped'
            WHEN 3 THEN N'Cancelled'
            WHEN 4 THEN N'On Hold'
            WHEN 5 THEN N'Completed'
            ELSE N'Unknown'
        END
    );
END;
GO
