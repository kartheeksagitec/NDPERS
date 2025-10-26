CREATE TABLE [dbo].[SGW_BPM_CASE_INST_EXEC_PATH] (
    [EXECUTION_PATH_ID]           INT           IDENTITY (1, 1) NOT NULL,
    [CASE_INSTANCE_ID]            INT           NOT NULL,
    [ELEMENT_ID]                  VARCHAR (50)  NOT NULL,
    [STATUS_ID]                   INT           NOT NULL,
    [STATUS_VALUE]                VARCHAR (4)   NOT NULL,
    [ACTIVITY_INSTANCE_ID]        INT           NULL,
    [PARENT_ACTIVITY_INSTANCE_ID] INT           NULL,
	[INITIATED_ACTIVITY_INSTANCE_ID] INT NULL,
    [NAME]                        VARCHAR (MAX) NULL,
    [START_DATE]                  DATETIME      NULL,
    [END_DATE]                    DATETIME      NULL,
    [PARAMETERS_SNAPSHOT]         VARBINARY(MAX)         NULL,
    [ELEMENT_TYPE_ID]             INT           NULL,
    [ELEMENT_TYPE_VALUE]          VARCHAR (4)   NULL,
    CONSTRAINT [PK_SGW_BPM_CASE_INST_EXEC_PATH] PRIMARY KEY CLUSTERED ([EXECUTION_PATH_ID] ASC)
);

