USE [master]
GO
CREATE LOGIN [order] WITH PASSWORD=N'order', DEFAULT_DATABASE=[ModularMonolith], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [ModularMonolith]
GO
CREATE USER [order] FOR LOGIN [order]
ALTER USER [order] WITH DEFAULT_SCHEMA=[Order]
ALTER AUTHORIZATION ON SCHEMA::[Order] TO [order]
GO

-- Replace [USER] to username, PASSWORD to passwod, [DATABASE] to database name, [SCHEMA] to schema
-- USE [master]
-- GO
-- CREATE LOGIN [USER] WITH PASSWORD=N'PASSWORD', DEFAULT_DATABASE=[DATABASE], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
-- GO
-- USE [DATABASE]
-- GO
-- CREATE USER [USER] FOR LOGIN [USER]
-- ALTER USER [USER] WITH DEFAULT_SCHEMA=[SCHEMA]
-- ALTER AUTHORIZATION ON SCHEMA::[SCHEMA] TO [USER]
-- GO
