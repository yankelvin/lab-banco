IF NOT EXISTS (SELECT *
FROM sys.schemas
WHERE   name = N'Locadora' )
    EXEC('CREATE SCHEMA [Locadora]');
GO
