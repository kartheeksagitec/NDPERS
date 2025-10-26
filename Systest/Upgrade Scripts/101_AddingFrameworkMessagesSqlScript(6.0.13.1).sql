-----------------------------------------------------------------------------------------------------------------------
--Created By	:	Siddharth Sabadra
--Created On	:	6th October 2020
--Description	:	Adding Message to sgs_fwk_messages table
------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS(select * from sgs_fwk_messages where MESSAGE_ID=1564) 
 insert into sgs_fwk_messages values(1564,null,'This Process has a dependent process in Progess and is Restricted',16,'E','FWK',256,'HDEM','system',getdate(),'FRAMEWORK',getdate(),0)