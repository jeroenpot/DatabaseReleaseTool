
-- Table_Create

DECLARE @table VARCHAR(256)
set @table = $(tablename)

PRINT 'Create table [dbo].' + @table
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
BEGIN
	EXEC('CREATE TABLE [dbo].[' + @table + '] ([autonummer] INT IDENTITY NOT NULL, CONSTRAINT [PK_' + @table + '] PRIMARY KEY CLUSTERED ([autonummer] asc)) on [PRIMARY]')
END ELSE BEGIN
	PRINT('-- Table [dbo].[' + @table + '] already exists.')
END


