IF NOT EXISTS (SELECT *
FROM sys.schemas
WHERE   name = N'Vendas' )
    EXEC('CREATE SCHEMA [Vendas]');
GO
