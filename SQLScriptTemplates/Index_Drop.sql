
DECLARE @table VARCHAR(256)
set @table = 'xxx'
DECLARE @index VARCHAR(256)
set @index = 'zzz'

PRINT 'Delete index [' + @index + '] for table [' + @table + ']'
IF EXISTS (
	SELECT * FROM sys.indexes i 
	INNER JOIN sys.objects o on i.object_id = o.object_id
	WHERE i.name = @index AND o.name = @table
)
BEGIN
	EXEC ('DROP INDEX [' + @index + '] ON [' + @table + '] WITH ( ONLINE = OFF )')
END 



