------------------------------------------------------------------------------------------------------------------------ 
--Created By	:	Rashmi Deepak
--Created On	:	14th September 2020
--Description	:	Adding audit columns in  SGW_BPM_CASE_EXCPT_NTFC
------------------------------------------------------------------------------------------------------------------------ 

IF COL_LENGTH('dbo.SGW_BPM_CASE_EXCPT_NTFC', 'CREATED_BY') IS NULL
BEGIN
ALTER TABLE SGW_BPM_CASE_EXCPT_NTFC
ADD CREATED_BY varchar(50) NOT NULL DEFAULT 'BPM Service'
END
GO

IF COL_LENGTH('dbo.SGW_BPM_CASE_EXCPT_NTFC', 'CREATED_DATE') IS NULL
BEGIN
ALTER TABLE SGW_BPM_CASE_EXCPT_NTFC
ADD CREATED_DATE datetime NOT NULL DEFAULT getdate()
END
GO

IF COL_LENGTH('dbo.SGW_BPM_CASE_EXCPT_NTFC', 'MODIFIED_BY') IS NULL
BEGIN
ALTER TABLE SGW_BPM_CASE_EXCPT_NTFC
ADD MODIFIED_BY varchar(50) NOT NULL DEFAULT 'BPM Service'
END
GO

IF COL_LENGTH('dbo.SGW_BPM_CASE_EXCPT_NTFC', 'MODIFIED_DATE') IS NULL
BEGIN
ALTER TABLE SGW_BPM_CASE_EXCPT_NTFC
ADD MODIFIED_DATE datetime NOT NULL DEFAULT getdate()
END
GO

IF COL_LENGTH('dbo.SGW_BPM_CASE_EXCPT_NTFC', 'UPDATE_SEQ') IS NULL
BEGIN
ALTER TABLE SGW_BPM_CASE_EXCPT_NTFC
ADD UPDATE_SEQ int NOT NULL DEFAULT 0
END
GO