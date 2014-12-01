
-- View_Drop

DECLARE @view VARCHAR(256)
set @view = 'zzz'

PRINT 'Delete view [dbo].[' + @view + ']'
IF EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @view and TYPE in ('V ')
)
BEGIN
	EXECUTE('drop view [dbo].[' + @view + ']')
END ELSE BEGIN
	PRINT '-- View [dbo].[' + @view + '] was already deleted'
END
