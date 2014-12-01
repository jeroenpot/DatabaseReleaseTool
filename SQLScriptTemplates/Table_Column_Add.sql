
-- Table_Column_Add

DECLARE @table VARCHAR(256)
set @table = 'TABLENAME_'
DECLARE @column VARCHAR(256)
set @column = 'bbbbb'
DECLARE @columnSpec VARCHAR(256)
set @columnSpec = 'int'
DECLARE @columnDefault VARCHAR(256)
SET @columnDefault = ''

PRINT 'Add column [' + @column + '] to table [' + @table + ']'
IF NOT EXISTS ( SELECT 1 FROM sys.columns c
	INNER JOIN sys.tables t ON c.object_id = t.object_id
	WHERE t.name = @table AND c.name = @column
)
BEGIN
	IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
	BEGIN
		EXEC('ALTER TABLE [dbo].[' + @table + '] ADD [' + @column + '] ' + @columnSpec) 

		IF @columnDefault != ''
		BEGIN
			EXEC ('ALTER TABLE [dbo].[' + @table + '] ADD CONSTRAINT [DF_' + @table + '_' + @column + '] DEFAULT ' + @columnDefault + ' FOR [' + @column + '] WITH VALUES')
		END
	END ELSE BEGIN
        declare @error VARCHAR(256)
        SET @error = '-- The table [' + @table + '] does not exists in this database'
		RAISERROR(@error, 16, 1)
	END
END ELSE BEGIN
	PRINT '-- Column [' + @column + '] in table [' + @table + '] already exists'
END