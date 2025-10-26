--------------Created By: Sanket Chougale
--------------Purpose   : PIR 26109
--------------Date      : 29/11/2023

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'DETAIL_COMMENTS' AND Object_ID = Object_ID(N'dbo.SGT_EMPLOYER_PAYROLL_HEADER'))
BEGIN
ALTER TABLE SGT_EMPLOYER_PAYROLL_HEADER ADD DETAIL_COMMENTS VARCHAR(1) NULL
END
GO
