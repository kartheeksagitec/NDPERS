CREATE TABLE [dbo].[SGW_BPM_CASE_INSTANCE_TRACING] (
    [CASE_ID]              INT           NULL,
    [PROCESS_ID]           INT           NULL,
    [ACTIVITY_ID]          INT           NULL,
    [CASE_INSTANCE_ID]     INT           NULL,
    [PROCESS_INSTANCE_ID]  INT           NULL,
    [ACTIVITY_INSTANCE_ID] INT           NULL,
    [TRACE_DETAILS]        VARCHAR (MAX) NULL,
    [ERROR_DETAILS]        VARCHAR (MAX) NULL,
    [CREATED_BY]           VARCHAR (50)  NOT NULL,
    [CREATED_DATE]         DATETIME      NOT NULL,
	[MACHINE_NAME]		   NVARCHAR(MAX) NULL
);

