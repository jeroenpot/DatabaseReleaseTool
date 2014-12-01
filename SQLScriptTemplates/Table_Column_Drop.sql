
-- Table_Column_Drop

DECLARE @table VARCHAR(256)
set @table = 'xxx'
DECLARE @column VARCHAR(256)
set @column = 'yyy'

PRINT 'Deleting Column [' + @column + '] in table [' + @table + ']'
IF EXISTS ( SELECT 1 FROM sys.columns c
	INNER JOIN sys.tables t ON c.object_id = t.object_id
	WHERE t.name = @table AND c.name = @column
)
BEGIN
	DECLARE @name VARCHAR(256)
	SELECT @name = d.name
	FROM sys.columns c
	INNER JOIN sys.tables t 
		ON c.object_id = t.object_id
	INNER JOIN sys.default_constraints d
		ON c.default_object_id = d.object_id
	WHERE t.name = @table AND c.name = @column

	IF NOT @name IS NULL
	BEGIN
		PRINT 'First dropping constraint ' + @name
		EXEC('ALTER TABLE [dbo].[' + @table + '] DROP CONSTRAINT ' + @name)
	END

	EXEC ('ALTER TABLE [dbo].[' + @table + '] DROP COLUMN [' + @column + ']')
END ELSE BEGIN
	IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
	BEGIN
		PRINT '-- Column [' + @column + '] in table [' + @table + '] already deleted'
	END ELSE BEGIN
		PRINT '-- Table [' + @table + '] does not exist in database'
	END
END
