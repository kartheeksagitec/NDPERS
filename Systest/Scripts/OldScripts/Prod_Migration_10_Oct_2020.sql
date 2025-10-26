/********************Purpose: PIR 22763 ******************************
*********************Created By: Ganesh********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT * FROM SGS_COR_TEMPLATES WHERE TEMPLATE_NAME ='PER-0357')
INSERT INTO [dbo].[SGS_COR_TEMPLATES]
           ([TEMPLATE_NAME]
           ,[TEMPLATE_DESC]
           ,[TEMPLATE_GROUP_ID]
           ,[TEMPLATE_GROUP_VALUE]
           ,[ACTIVE_FLAG]
           ,[DESTINATION_ID]
           ,[DESTINATION_VALUE]
           ,[ASSOCIATED_FORMS]
           ,[FILTER_OBJECT_ID]
           ,[FILTER_OBJECT_FIELD]
           ,[FILTER_OBJECT_VALUE]
           ,[CONTACT_ROLE_ID]
           ,[CONTACT_ROLE_VALUE]
           ,[BATCH_FLAG]
           ,[ONLINE_FLAG]
           ,[AUTO_PRINT_FLAG]
           ,[PRINTER_NAME_ID]
           ,[PRINTER_NAME_VALUE]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ]
           ,[IMAGE_DOC_CATEGORY_ID]
           ,[IMAGE_DOC_CATEGORY_VALUE]
           ,[FILENET_DOCUMENT_TYPE_ID]
           ,[FILENET_DOCUMENT_TYPE_VALUE]
           ,[DOCUMENT_CODE])
VALUES
		   ('PER-0357','SFN 19789 LEP IBS Billing Statement',19,'MMBR','Y',601,null,'wfmPaymentElectionAdjustmentMaintenance;wfmPersonMaintenance;',NULL,NULL,NULL,515,NULL,'N','Y','N',44,null,'PIR 22763',GETDATE(),'PIR 22763',GETDATE(),0,603,'MEMB',604,'FORM','19789')
GO

/********************Purpose: PIR 18493 ******************************
*********************Created By: Abhijeet********************************
*********************Comments: Updated the language option in dropdown *****************/
UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'View the status of my Application' WHERE CODE_SERIAL_ID =18058 AND CODE_ID = 7014 AND CODE_VALUE = 'VSRA' AND DESCRIPTION = 'View the status of Retirement Benefit Application' 
/*********************Added ‘Other Documents’ (without Document Code) to SGT_DOCUMENT table for MSS*********************/
SET IDENTITY_INSERT [dbo].[SGT_DOCUMENT] ON
IF NOT EXISTS (SELECT 1 FROM [SGT_DOCUMENT] WHERE DOCUMENT_NAME = 'Other Documents') -- 682
INSERT [SGT_DOCUMENT]
		(DOCUMENT_ID,DOCUMENT_CODE, DOCUMENT_NAME, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ, IGNORE_PROCESS_FLAG, WSS, FILENET_DOCUMENT_TYPE) 
VALUES (682,'', 'Other Documents', 'PIR 18493', GETDATE(), 'PIR 18493', GETDATE(), 0,'Y','MSS', 'FORM' )
GO
SET IDENTITY_INSERT [dbo].[SGT_DOCUMENT] OFF

/********************************************************************************
************************ SYSTEM PATH FOR UPLOADED DOCS****************************
*********************************************************************************/
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_PATHS WHERE PATH_CODE = 'MSSDocs')
INSERT [SGS_SYSTEM_PATHS] ([PATH_CODE], [PATH_VALUE], [PATH_DESCRIPTION], [CREATED_BY], 
[CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) VALUES 
('MSSDocs', 'MSSDocumentUpload', 'Stores the other documents where uploaded documents through WSS reside', 'PIR 18493', GETDATE(), 'PIR 18493', GETDATE(), 0)
GO