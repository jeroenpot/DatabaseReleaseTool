
-- Trigger_Drop

DECLARE @trigger VARCHAR(256)
set @trigger = 'zzz'

PRINT 'Create / Change trigger [dbo].[' + @trigger + ']'
IF EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @trigger and TYPE = 'TR'
)
BEGIN
	EXECUTE('drop trigger [dbo].[' + @trigger + ']')
END ELSE BEGIN
	PRINT '-- Trigger [dbo].[' + @trigger + '] was already deleted'
END