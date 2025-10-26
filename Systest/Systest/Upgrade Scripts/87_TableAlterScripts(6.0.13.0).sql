
declare @Setting_value varchar(1000)='10000'
INSERT INTO [dbo].[SGS_SYSTEM_SETTINGS]
           ([SETTING_NAME]
           ,[SETTING_TYPE]
           ,[SETTING_VALUE]
           ,[ENCRYPTED_FLAG]
		   ,[REFRESHABLE])
     VALUES
           ('DBCallDelayForSettingsRefresh'
           ,'int'
           ,@Setting_value
           ,null
		   ,'Y');
GO
declare @Setting_value1 varchar(1000)='true'
INSERT INTO [dbo].[SGS_SYSTEM_SETTINGS]
           ([SETTING_NAME]
           ,[SETTING_TYPE]
           ,[SETTING_VALUE]
           ,[ENCRYPTED_FLAG]
		   ,[REFRESHABLE])
     VALUES
           ('SupportsAlwaysOnFlag'
           ,'bool'
           ,@Setting_value1
           ,null
		   ,'Y');
GO
