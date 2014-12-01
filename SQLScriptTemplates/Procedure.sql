
-- procedure

DECLARE @procedure VARCHAR(256)
set @procedure = 'zzz' 

PRINT 'Create / update stored procedure [dbo].[' + @procedure + ']'
IF NOT EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @procedure and TYPE in ('P ')
)
BEGIN
	EXECUTE('create procedure [dbo].[' + @procedure + ']  as begin select 1 end')
END

EXECUTE('grant execute on [dbo].[' + @procedure + '] to Execute_ALL_SP')

EXECUTE('
ALTER PROCEDURE [dbo].[' + @procedure + ']
AS
BEGIN
	SELECT 1
END
')