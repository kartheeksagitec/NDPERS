------------------------------------------------------------------------------------------------------------------------ 
--Created By	:	Rashmi Deepak
--Created On	:	14th September 2020
--Description	:	Adding audit columns in  SGW_BPM_SERVICE_TIMER
------------------------------------------------------------------------------------------------------------------------ 

IF COL_LENGTH('dbo.SGW_BPM_SERVICE_TIMER', 'CREATED_BY') IS NULL
BEGIN
ALTER TABLE SGW_BPM_SERVICE_TIMER
ADD CREATED_BY varchar(50) NOT NULL DEFAULT 'BPM Service'
END
GO

IF COL_LENGTH('dbo.SGW_BPM_SERVICE_TIMER', 'CREATED_DATE') IS NULL
BEGIN
ALTER TABLE SGW_BPM_SERVICE_TIMER
ADD CREATED_DATE datetime NOT NULL DEFAULT getdate()
END
GO

IF COL_LENGTH('dbo.SGW_BPM_SERVICE_TIMER', 'MODIFIED_BY') IS NULL
BEGIN
ALTER TABLE SGW_BPM_SERVICE_TIMER
ADD MODIFIED_BY varchar(50) NOT NULL DEFAULT 'BPM Service'
END
GO

IF COL_LENGTH('dbo.SGW_BPM_SERVICE_TIMER', 'MODIFIED_DATE') IS NULL
BEGIN
ALTER TABLE SGW_BPM_SERVICE_TIMER
ADD MODIFIED_DATE datetime NOT NULL DEFAULT getdate()
END
GO

IF COL_LENGTH('dbo.SGW_BPM_SERVICE_TIMER', 'UPDATE_SEQ') IS NULL
BEGIN
ALTER TABLE SGW_BPM_SERVICE_TIMER
ADD UPDATE_SEQ int NOT NULL DEFAULT 0
END
GO