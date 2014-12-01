
-- Function_Drop

DECLARE @function VARCHAR(256)
set @function = 'zzz'

PRINT 'Create / Update of function [dbo].[' + @function + ']'
IF EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @function and TYPE in ('AF','FN','FS','FT','IF','TF')
)
BEGIN
	EXECUTE('drop function [dbo].[' + @function + ']')
END ELSE BEGIN
	PRINT '-- Function [dbo].[' + @function + '] was already deleted'
END
