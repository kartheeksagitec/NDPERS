/********************Purpose: PIR 25924 ******************************
*********************Created By:  Sarvesh, Ghante********************************
*********************Comments: *****************/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'COMMENT' AND Object_ID = Object_ID(N'dbo.SGT_PERSON_TFFR_TIAA_SERVICE'))
BEGIN
    ALTER TABLE SGT_PERSON_TFFR_TIAA_SERVICE
    ADD COMMENT VARCHAR(500) NULL
END
GO