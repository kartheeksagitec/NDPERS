
--- Adding IS_BROWSERCLOSE column in SGS_ACT_LOG_INST table
IF NOT EXISTS ( SELECT SO.NAME, SC.NAME 
            FROM SYSOBJECTS SO 
            JOIN SYSCOLUMNS SC ON SO.id = SC.ID 
            JOIN SYSTYPES ST ON SC.XUSERTYPE = ST.XUSERTYPE
            WHERE SO.name = 'SGS_ACT_LOG_INST' 
            AND SC.Name = 'IS_BROWSERCLOSE' 
            AND SO.xtype = 'U')
BEGIN
  PRINT 'Adding column IS_BROWSERCLOSE varchar(1)'
  ALTER TABLE SGS_ACT_LOG_INST
ADD IS_BROWSERCLOSE varchar(1) NULL
END
ELSE
BEGIN
PRINT 'Column IS_BROWSERCLOSE already exists in SGS_ACT_LOG_INST Table'
END
GO



--- Adding BROWSERCLOSE_TIME column in SGS_ACT_LOG_INST table
IF NOT EXISTS ( SELECT SO.NAME, SC.NAME 
            FROM SYSOBJECTS SO 
            JOIN SYSCOLUMNS SC ON SO.id = SC.ID 
            JOIN SYSTYPES ST ON SC.XUSERTYPE = ST.XUSERTYPE
            WHERE SO.name = 'SGS_ACT_LOG_INST' 
            AND SC.Name = 'BROWSERCLOSE_TIME' 
            AND SO.xtype = 'U')
BEGIN
  PRINT 'Adding column BROWSERCLOSE_TIME DATETIME'
  ALTER TABLE SGS_ACT_LOG_INST
ADD BROWSERCLOSE_TIME  DATETIME NULL
END
ELSE
BEGIN
PRINT 'Column BROWSERCLOSE_TIME already exists in SGS_ACT_LOG_INST Table'
END
GO


/******Adding Table [dbo].[sgs_exception_log_detail]   ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[sgs_exception_log_detail](
       [exception_log_detail_id] [int] IDENTITY(1,1) NOT NULL,
       [exception_log_id] [int] NULL,
       [parameter_name] [varchar](100) NULL,
       [parameter_value] [varchar](max) NULL,
CONSTRAINT [PK_sgs_exception_log_detail] PRIMARY KEY CLUSTERED 
(
       [exception_log_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO