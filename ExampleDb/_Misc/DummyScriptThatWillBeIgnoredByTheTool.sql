
-- Table_Create

DECLARE @table VARCHAR(256)
set @table = 'Log'

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
BEGIN
	PRINT('Drop de tabel [dbo].[' + @table + ']')
	EXEC('DROP TABLE dbo.Log')
END

PRINT 'Aanmaken van de tabel [dbo].[' + @table + ']'
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = @table)
BEGIN
	EXEC('CREATE TABLE [dbo].[' + @table + '] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Hostname] [varchar] (255) NOT NULL,
	[Application] [varchar] (255) NULL,
	[ProcessId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Version] [varchar] (50) NULL,
	[Class] [varchar] (255) NOT NULL,
	[Method] [varchar] (255) NOT NULL,
	[LineNumber] [int] NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[ErrorCode] [int] NULL,	
	[Message] [varchar](4000) NOT NULL,
	[Exception] [varchar](2000) NULL
	) ON [PRIMARY]')
END ELSE BEGIN
	PRINT('-- De tabel [dbo].[' + @table + '] bestaat al.')
END