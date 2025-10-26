
/****** Object:  StoredProcedure [dbo].[GetSessionObject]    Script Date: 05-06-2020 19:20:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetSessionObject]
	-- Add the parameters for the stored procedure here
	 @SESSION_ID varchar(100) 
AS 
BEGIN
   --Insert statements for the stored procedure here
 SELECT SESSION_OBJECT FROM DBO.SGS_SESSION_DATA WITH(NOLOCK)  where SESSION_ID=@SESSION_ID 
END
GO




CREATE PROCEDURE [dbo].[UpdateSessionData]
	-- Add the parameters for the stored procedure here
	
	@SESSION_OBJECT varbinary(max),
	 @DATE_CREATED datetime ,
	 @SESSION_ID varchar(100) 
AS 
begin
   --Insert statements for the stored procedure here
   DECLARE @ROW_COUNT INT  ;
    set @ROW_COUNT = 0;
	SELECT @ROW_COUNT = 1 FROM  DBO.SGS_SESSION_DATA WHERE SESSION_ID = @SESSION_ID ;
	IF @ROW_COUNT =1 
			UPDATE DBO.SGS_SESSION_DATA SET SESSION_OBJECT = @SESSION_OBJECT, DATE_CREATED = @DATE_CREATED WHERE SESSION_ID = @SESSION_ID;
	ELSE 
			INSERT INTO DBO.SGS_SESSION_DATA (SESSION_ID,SESSION_OBJECT, DATE_CREATED) VALUES (@SESSION_ID,@SESSION_OBJECT, @DATE_CREATED)
END
GO



