-----------------------------------------
--Created By	:	Mansi Shah
--Created On	:	4th March 2020
--Description	:	Change datatype of image columns.
------------------------------------------------------------------------------------------------------------------------ 
IF (EXISTS (SELECT * FROM sys.columns WHERE NAME = 'PARAMETERS_SNAPSHOT' AND Object_ID = Object_ID('SGW_BPM_CASE_INST_EXEC_PATH')))
BEGIN
ALTER TABLE SGW_BPM_CASE_INST_EXEC_PATH ALTER COLUMN PARAMETERS_SNAPSHOT VARBINARY(MAX)
END
GO

IF (EXISTS (SELECT * FROM sys.columns WHERE NAME = 'OBJECT_VALUE' AND Object_ID = Object_ID('SGW_BPM_CASE_INST_PARAMETER')))
BEGIN
ALTER TABLE SGW_BPM_CASE_INST_PARAMETER ALTER COLUMN OBJECT_VALUE VARBINARY(MAX)
END
GO