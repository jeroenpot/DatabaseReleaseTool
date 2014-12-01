
DECLARE @table VARCHAR(256)
set @table = 'xxx'
DECLARE @index VARCHAR(256)
set @index = 'zzz'
DECLARE @indexFields VARCHAR(256)
set @indexFields = '[yyy] ASC'

PRINT 'Create index [' + @index + '] for table [' + @table + ']'
IF EXISTS (
	SELECT * FROM sys.indexes i 
	INNER JOIN sys.objects o on i.object_id = o.object_id
	WHERE i.name = @index AND o.name = @table
)
BEGIN
	PRINT 'Index [' + @index + '] for table [' + @table + '] already exists!'
END 
ELSE
BEGIN
	EXEC ('CREATE NONCLUSTERED INDEX [' + @index + '] ON [dbo].[' + @table + '] ( ' + @indexFields + ' ) ON [PRIMARY]')
END 

