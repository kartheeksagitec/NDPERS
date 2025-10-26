-----------------------------------------
--Created By	:	Siddharth Sabadra
--Created On	:	27st January 2020
--Description	:	Adding Message to SGS_FWK_MESSAGES table
------------------------------------------------------------------------------------------------------------------------ 
IF NOT EXISTS(SELECT * FROM SGS_MESSAGES where MESSAGE_ID = 20007034 and DISPLAY_MESSAGE = 'Please enter valid Process Events') 
 insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID, SEVERITY_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) values (20007034,'Please enter valid Process Events',20002001,'E','system',getdate(),'system',getdate(),0);

