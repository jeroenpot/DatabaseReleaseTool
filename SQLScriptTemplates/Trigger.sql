
-- Trigger

DECLARE @table VARCHAR(256)
SET @table = 'xxx' -- Attention, this name should also be uses in the Alter section
DECLARE @trigger VARCHAR(256)
SET @trigger = 'zzz' -- Attention, this name should also be uses in the Alter section

PRINT 'Create / change trigger [dbo].[' + @trigger + ']'
IF NOT EXISTS (SELECT 1 FROM sys.objects o
	inner join sys.sql_modules m ON o.object_id = m.object_id 
	WHERE [name] = @trigger and TYPE = 'TR'
)
BEGIN
	EXECUTE('create trigger [dbo].[' + @trigger + ']  on [' + @table + '] after insert as begin select 1 end')
END


ALTER trigger [dbo].[zzz]
	on [dbo].[xxx]
	after insert
as
begin
	SET NOCOUNT ON
	update xxx
	set xxx.idIsActive = 0
	from inserted
	where xxx.id = inserted.id 
		and xxx.idIsActive = 1 
		and inserted.autonummer != xxx.autonummer
	SET NOCOUNT OFF
end


