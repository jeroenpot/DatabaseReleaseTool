
-- Table_Column_Rename

DECLARE @table VARCHAR(256)
set @table = $(tablename)
DECLARE @column VARCHAR(256)
set @column = $(columnname)
DECLARE @columnNew VARCHAR(256)
set @columnNew = $(columnnamenew)

PRINT 'Alter name of column [' + @column + '] in table [' + @table + '] naar [' + @columnNew +']'
IF EXISTS ( SELECT 1 FROM sys.columns c
	INNER JOIN sys.tables t ON c.object_id = t.object_id
	WHERE t.name = @table AND c.name = @column
)
BEGIN
	IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
	BEGIN
		DECLARE @columnOld VARCHAR(256)
		SET @columnOld = '[dbo].[' + @table + '].[' + @column + ']'
		EXEC sp_rename @columnOld, @columnNew, 'COLUMN' 
	END ELSE BEGIN
		PRINT '-- Table [' + @table + '] does not exist in database'
	END
END ELSE BEGIN
	PRINT '-- Column [' + @column + '] in table [' + @table + '] does not exist'
END