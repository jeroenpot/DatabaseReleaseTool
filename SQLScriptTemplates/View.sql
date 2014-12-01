
-- view

DECLARE @view VARCHAR(256)
set @view = 'zzz' -- Attention, this name should also be uses in the Alter section

PRINT 'Create / Change view [dbo].[' + @view + ']'
IF NOT EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @view and TYPE in ('V ')
)
BEGIN
	EXECUTE('create view [dbo].[' + @view + ']  as select 1 a')
END


ALTER view [dbo].[zzz]
AS
	SELECT 1 b




