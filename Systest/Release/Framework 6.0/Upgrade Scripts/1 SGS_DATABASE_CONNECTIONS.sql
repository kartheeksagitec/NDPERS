
/****** Object:  Table [dbo].[SGS_DATABASE_CONNECTIONS]    Script Date: 11/20/2017 12:23:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGS_DATABASE_CONNECTIONS](
	[DATABASE_CONNECTION_ID] [int] IDENTITY(1,1) NOT NULL,
	[CONNECTION_NAME] [varchar] (50) NOT NULL,
	[CONNECTION_TYPE] [varchar] (50) NOT NULL,
	[CONNECTION_STRING] [varchar] (200) NOT NULL,
	[DBFACTOTY_PROVIDER] [varchar] (100) NOT NULL,
	[PASSWORD_ENCRYPTED_FLAG] [char] (1) NULL,
 CONSTRAINT [PK_SGS_DATABASE_CONNECTIONS] PRIMARY KEY CLUSTERED 
(
	[DATABASE_CONNECTION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Unique id for the table.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'DATABASE_CONNECTION_ID'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Unique name of database connection.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'CONNECTION_NAME'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Type of connection E.g. SqlServerClient' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'CONNECTION_TYPE'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Connection string for database.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'CONNECTION_STRING'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Name of class from where we use objects to connect database. E.g. System.Data.SqlClient' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'DBFACTOTY_PROVIDER'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Flag indicates whether the password is encrypted.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS', @level2type=N'COLUMN',@level2name=N'PASSWORD_ENCRYPTED_FLAG'
GO

EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'This table stores information about the database connections for the system' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SGS_DATABASE_CONNECTIONS'
GO


