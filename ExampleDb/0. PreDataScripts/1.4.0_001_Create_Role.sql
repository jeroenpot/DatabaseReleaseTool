declare @role varchar(50)
set @role = 'Execute_ALL_SP'
if not exists (select 1 from sys.database_principals where name=@role and Type = 'R')
begin
	EXEC('create role ' + @role)
end
