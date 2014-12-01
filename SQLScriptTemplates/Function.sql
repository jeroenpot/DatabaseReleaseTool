
-- Function

DECLARE @function VARCHAR(256)
set @function = 'zzz' -- Attention, This name should also be used with the Alter section

PRINT 'Create / Update of function [dbo].[' + @function + ']'
IF NOT EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @function and TYPE in ('AF','FN','FS','FT','IF','TF')
)
BEGIN
	EXECUTE('create function [dbo].[' + @function + '] () returns int as begin return 0 end')
	EXECUTE('grant execute on [dbo].[' + @function + '] to Execute_ALL_SP')

END

ALTER FUNCTION [dbo].[zzz] 
	(@myvar varchar(2)) 
	RETURNS char(1) AS
BEGIN
	DECLARE @returnValue char(10)

	set @returnValue = @myvar + @myvar

	RETURN @returnValue
END


