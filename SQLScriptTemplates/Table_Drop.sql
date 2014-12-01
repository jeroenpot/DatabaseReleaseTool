
-- Table_Drop

DECLARE @table VARCHAR(256)
set @table = $(tablename)

PRINT 'Delete table [dbo].' + @table
IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
BEGIN
	EXEC('DROP TABLE [dbo].[' + @table + ']')
END ELSE BEGIN
	PRINT('-- Table [dbo].[' + @table + '] already dropped.')
END