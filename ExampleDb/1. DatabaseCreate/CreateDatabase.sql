IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ExampleDb')
BEGIN
	CREATE DATABASE [ExampleDb] ON  PRIMARY 
	( NAME = N'ExampleDb', FILENAME = N'D:\Databases\ExampleDb.mdf' , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
	LOG ON 
	( NAME = N'ExampleDb_log', FILENAME = N'D:\Databases\ExampleDb_log.ldf' , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END
ELSE
BEGIN
	PRINT 'Database: [ExampleDb] already exists'
END

