
-- Table_FK_add

DECLARE @table1 VARCHAR(256)
DECLARE @table2 VARCHAR(256)
DECLARE @FK_Constrain VARCHAR(256)
DECLARE @kolom VARCHAR(256)

set @table1 = ''
set @table2 = ''
set @FK_Constrain = ''
set @kolom = ''

PRINT 'Alter table [dbo].' + @table1
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = @FK_Constrain)
BEGIN
	EXEC('ALTER TABLE [dbo].[' + @table1 + '] ADD CONSTRAINT ' + @FK_Constrain + 
         ' FOREIGN KEY (' + @kolom + ') REFERENCES [dbo].[' + @table2 + ']' 
         + '(' + @kolom + ')') 
END ELSE 
BEGIN
	PRINT('-- The FK constraint ' + @FK_Constrain + ' already exists.')
END