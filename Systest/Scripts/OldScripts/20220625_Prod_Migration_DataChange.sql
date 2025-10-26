IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_BPM_SERVICE_TIMER] where ACTION_METHOD = 'ResumeStuckInstances')
BEGIN
INSERT INTO [dbo].[SGW_BPM_SERVICE_TIMER]
           ([ACTION_METHOD]
           ,[INTERVAL]
           ,[ENABLED]
           ,[BPM_TRACING_FLAG]
           ,[BPM_QUERY_TRACING_FLAG]
           ,[BPM_QUERY_LIMIT]
           ,[BPM_QUERY_LIMIT_ALERT]
           ,[BPM_QUERY_HIGH_LIMIT]
           ,[BPM_QUERY_HIGH_LIMIT_ALERT]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('ResumeStuckInstances'
           ,120000
           ,'Y'
           ,'Y'
           ,'Y'
           ,3000
           ,NULL
           ,3000
           ,NULL
           ,'BPM_Upgrade'
           ,GETDATE()
           ,'BPM_Upgrade'
           ,GETDATE()
           ,0)
END
GO


