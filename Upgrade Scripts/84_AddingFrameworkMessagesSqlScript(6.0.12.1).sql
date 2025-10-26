-----------------------------------------
--Created By	:	Siddharth Sabadra
--Created On	:	10th July 2020
--Description	:	Adding Message to sgs_fwk_messages table
------------------------------------------------------------------------------------------------------------------------ 
IF NOT EXISTS(select * from sgs_fwk_messages where MESSAGE_ID=1561) 
 insert into sgs_fwk_messages values(1561,null,'Could not find the path to upload documents',16,'E','FWK',256,'HDEM','system',getdate(),'FRAMEWORK',getdate(),0)

IF NOT EXISTS(select * from sgs_fwk_messages where MESSAGE_ID=1562) 
 insert into sgs_fwk_messages values(1562,null,'No BPM Process find to upload a file',16,'E','FWK',256,'HDEM','system',getdate(),'FRAMEWORK',getdate(),0)

IF NOT EXISTS(select * from sgs_fwk_messages where MESSAGE_ID=1563) 
 insert into sgs_fwk_messages values(1563,null,'No file chosen to upload. Please choose one before proceeding',16,'E','FWK',256,'HDEM','system',getdate(),'FRAMEWORK',getdate(),0)



 