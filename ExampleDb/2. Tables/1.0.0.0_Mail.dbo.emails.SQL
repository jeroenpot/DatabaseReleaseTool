SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

       IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[emails]'))
	    BEGIN
		    IF EXISTS (select * from syscolumns where name like '@mailto')
			    BEGIN		
				    ALTER TABLE dbo.emails
				    ALTER COLUMN mailto varchar (100) NOT NULL
			    END
		    IF EXISTS (select * from syscolumns where name like '@mailfrom')
			    BEGIN
				    ALTER TABLE dbo.emails
				    ALTER COLUMN mailfrom varchar (100) NOT NULL
			    END
		    IF EXISTS (select * from syscolumns where name like '@replyto')
			    BEGIN
				    ALTER TABLE dbo.emails
				    ALTER COLUMN  replyto varchar (100) NOT NULL
			    END
		    IF EXISTS (select * from syscolumns where name like '@subject')
			    BEGIN		
				    ALTER TABLE dbo.emails
				    ALTER COLUMN  subject varchar (100) NOT NULL
		     END
		   
	    END

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[emails]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[emails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[mailto] [varchar](100) NOT NULL,
	[mailfrom] [varchar](100) NOT NULL,
	[replyto] [varchar](100) NULL,
	[subject] [varchar](100) NULL,
	[body] [text] NULL,
	[send] [tinyint] NULL,
 CONSTRAINT [PK_emails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END


SET ANSI_PADDING OFF


/****** Object:  Table [dbo].[Attachement]    Script Date: 09/21/2007 09:45:04 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING OFF

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attachement]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Attachement](
	[attachementId] [int] IDENTITY(1,1) NOT NULL,
	[emailid] [int] NOT NULL,
	[Attachement] [image] NOT NULL,
	[bestandsnaam] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Attachement] PRIMARY KEY CLUSTERED 
(
	[attachementId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

SET ANSI_PADDING OFF

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Attachement_Attachement]') AND parent_object_id = OBJECT_ID(N'[dbo].[Attachement]'))
ALTER TABLE [dbo].[Attachement]  WITH CHECK ADD  CONSTRAINT [FK_Attachement_Attachement] FOREIGN KEY([attachementId])
REFERENCES [dbo].[Attachement] ([attachementId])

ALTER TABLE [dbo].[Attachement] CHECK CONSTRAINT [FK_Attachement_Attachement]

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Attachement_emails]') AND parent_object_id = OBJECT_ID(N'[dbo].[Attachement]'))
ALTER TABLE [dbo].[Attachement]  WITH CHECK ADD  CONSTRAINT [FK_Attachement_emails] FOREIGN KEY([emailid])
REFERENCES [dbo].[emails] ([id])
ON DELETE CASCADE

ALTER TABLE [dbo].[Attachement] CHECK CONSTRAINT [FK_Attachement_emails]


IF NOT EXISTS (select * from syscolumns inner join sysobjects on syscolumns.[id] = sysobjects.id where syscolumns.name = 'inserted' and sysobjects.name = 'emails')
    BEGIN		
	    ALTER TABLE dbo.emails
	    ADD inserted DateTime NOT NULL DEFAULT GetDate()
 END