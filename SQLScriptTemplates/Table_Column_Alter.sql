
-- Table_Column_Alter

DECLARE @table VARCHAR(256)
set @table = $(tablename)
DECLARE @column VARCHAR(256)
set @column = $(columnname)
DECLARE @columnSpec VARCHAR(256)
set @columnSpec = $(columnspec)
DECLARE @columnDefault VARCHAR(256)
SET @columnDefault = $(columndefault) 

PRINT 'Alter Column [' + @column + '] in table [' + @table + ']'
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

	EXEC ('ALTER TABLE [dbo].[' + @table + '] ALTER COLUMN [' + @column + '] ' + @columnSpec)

	IF LEN(@columnDefault) > 0
	BEGIN
		EXEC ('ALTER TABLE [dbo].[' + @table + '] ADD CONSTRAINT [DF_' + @table + '_' + @column + '] DEFAULT ' + @columnDefault + ' FOR [' + @column + '] WITH VALUES')
	END
END ELSE BEGIN
    declare @error VARCHAR(256)
	IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
	BEGIN
        SET @error = '-- Column [' + @column + '] in table [' + @table + '] does not exist'
		RAISERROR(@error, 16, 1)
	END ELSE BEGIN
        SET @error = '-- Table [' + @table + '] does not exist in this database'
		RAISERROR(@error, 16, 1)
	END
END