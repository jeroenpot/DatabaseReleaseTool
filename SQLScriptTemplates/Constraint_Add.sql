
-- Constraint_Add

DECLARE @constraintName VARCHAR(256)
set @constraintName  = 'FK_'
DECLARE @table VARCHAR(256)
set @table = 'TABLE_NAME'

PRINT 'Add constraint [' + @constraintName + '] on table [' + @table + ']'
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE [CONSTRAINT_NAME] = @constraintName AND [TABLE_NAME] = @table )
BEGIN
	PRINT '-- Constraint [' + @constraintName + '] on table [' + @table + '] already exists'
END ELSE BEGIN

	PRINT '-- Constraint [' + @constraintName + '] is added to table [' + @table + '] '

END