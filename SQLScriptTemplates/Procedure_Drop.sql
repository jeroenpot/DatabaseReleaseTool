
-- Procedure_Drop

DECLARE @procedure VARCHAR(256)
set @procedure = 'zzz'

PRINT 'Delete stored procedure [dbo].[' + @procedure + ']'
IF EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @procedure and TYPE in ('P ')
)
BEGIN
	EXECUTE('drop procedure [dbo].[' + @procedure + ']')
END ELSE BEGIN
	PRINT '-- Stored procedure [dbo].[' + @procedure + '] was already deleted'
END
