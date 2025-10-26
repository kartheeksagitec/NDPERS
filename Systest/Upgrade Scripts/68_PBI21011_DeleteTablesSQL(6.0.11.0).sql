-----------------------------------------
--Created By	:	Mansi Shah
--Created On	:	4th March 2020
--Description	:	Removal of unwanted BPM tables.
------------------------------------------------------------------------------------------------------------------------ 
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE  TABLE_NAME = 'SGW_BPM_CASE_PARAMETER'))
BEGIN
drop table SGW_BPM_CASE_PARAMETER
END
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE  TABLE_NAME = 'SGW_BPM_PROCESS_SEQUENCE_FLOW'))
BEGIN
drop table SGW_BPM_PROCESS_SEQUENCE_FLOW
END
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE  TABLE_NAME = 'SGW_BPM_CASE_MESSAGE_FLOW'))
BEGIN
drop table SGW_BPM_CASE_MESSAGE_FLOW
END
GO