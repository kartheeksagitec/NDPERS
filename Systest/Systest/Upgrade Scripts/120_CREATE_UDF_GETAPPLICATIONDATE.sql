CREATE FUNCTION [dbo].[UDF_GETAPPLICATIONDATE]()
RETURNS DateTime
AS
BEGIN
	
	-- Return the result of the function
	Declare @Region varchar(4);
	Declare @UseApplicationDate varchar(1);
	Declare @ApplicationDate DateTime;
	DECLARE @LocalApplicationDate DateTime;	
	Select @Region = REGION_VALUE,@UseApplicationDate=USE_APPLICATION_DATE,@ApplicationDate=APPLICATION_DATE from SGS_SYSTEM_MANAGEMENT;

	IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='SGS_TIME_TRAVEL') 
		SELECT @LocalApplicationDate = APPLICATION_DATE FROM SGS_TIME_TRAVEL WHERE MACHINE_NAME = (SELECT HOST_NAME())


	IF(@LocalApplicationDate IS NOT NULL and @Region = 'DEVL' AND @UseApplicationDate IS NOT NULL AND @UseApplicationDate = 'Y')
		RETURN CONVERT(DATETIME, CONVERT(CHAR(8), @LocalApplicationDate, 112) + ' ' + FORMAT(GETDATE(), 'HH:mm:ss.fff'));	
	
	ELSE IF(@Region <> 'PROD' AND @UseApplicationDate IS NOT NULL AND @UseApplicationDate = 'Y' AND @ApplicationDate IS NOT NULL)
		RETURN CONVERT(DATETIME, CONVERT(CHAR(8), @ApplicationDate, 112) + ' ' + FORMAT(GETDATE(), 'HH:mm:ss.fff'));
				
	RETURN GETDATE();
END