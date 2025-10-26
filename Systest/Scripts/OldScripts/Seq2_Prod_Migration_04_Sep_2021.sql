
/********************Purpose: PIR 23966******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: 23966 *****************/
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 10424)
	INSERT [SGS_MESSAGES] ([MESSAGE_ID],[DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], 
							[CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
	VALUES (10424,N'Enrollment start date cannot be earlier than the employment start date', 16, N'E', NULL, NULL, N'PIR 23966', GETDATE(), N'PIR 23966', GETDATE(), 0)
GO

/********************Purpose: PIR 18484 New Template for Person Maintenance (SFN-61324)******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: Person Maintenance (SFN-61324) *****************/

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_COR_TEMPLATES] WHERE TEMPLATE_NAME = 'SFN-61324')
INSERT INTO [dbo].[SGS_COR_TEMPLATES] ([TEMPLATE_NAME], [TEMPLATE_DESC], [TEMPLATE_GROUP_ID], [TEMPLATE_GROUP_VALUE], [ACTIVE_FLAG], [DESTINATION_ID], [DESTINATION_VALUE], 
[ASSOCIATED_FORMS], [FILTER_OBJECT_ID], [FILTER_OBJECT_FIELD], [FILTER_OBJECT_VALUE], [CONTACT_ROLE_ID], [CONTACT_ROLE_VALUE], [BATCH_FLAG], [ONLINE_FLAG], [AUTO_PRINT_FLAG], [PRINTER_NAME_ID], 
[PRINTER_NAME_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ],
IMAGE_DOC_CATEGORY_ID,IMAGE_DOC_CATEGORY_VALUE,FILENET_DOCUMENT_TYPE_ID,FILENET_DOCUMENT_TYPE_VALUE,DOCUMENT_CODE) 
VALUES ( 'SFN-61324', 'SFN-61324 Authorization to Release Confidential Information', '19', 'MMBR', 'Y', '601', NULL, 
'wfmPersonMaintenance;', NULL, NULL, NULL, 
'515',NULL, 'N', 'Y', 'N', '44', NULL, 'PIR 18484', GETDATE(), 'PIR 18484', GETDATE(), '0',
603,'MEMB',604,'CORR','58770')
GO



/********************Purpose: PIR 24248
*********************Created By: Saylee P********************************
*********************Comments: Message text changed *****************/

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'Retirement Date must be before the Member’s age is 72.' WHERE MESSAGE_ID = 1939


/********************Purpose: PIR 23735******************************
*********************Created By: Saylee P********************************
*********************Comments: New code id created for Annual Enrollment months*****************/

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE] WHERE CODE_ID = 7024) 
BEGIN
INSERT INTO SGS_CODE (CODE_ID,DESCRIPTION,DATA1_CAPTION,DATA1_TYPE,DATA2_CAPTION,DATA2_TYPE,DATA3_CAPTION,DATA3_TYPE,FIRST_LOOKUP_ITEM,FIRST_MAINTENANCE_ITEM,COMMENTS,LEGACY_CODE_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(7024,'Annual Enrollment Month Range',N'ANNE Months', N'int ',NULL,NULL,NULL,NULL,'All',NULL,NULL,NULL,'PIR 23735',GETDATE(),'PIR 23735',GETDATE(),0)
END
GO 

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE CODE_ID = 7024) 
BEGIN
INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,DATA1,DATA2,DATA3,COMMENTS,START_DATE,END_DATE,CODE_VALUE_ORDER,LEGACY_CODE_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(7024,'ANNM','Annual Enrollment Months',3,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PIR 23735',GETDATE(),'PIR 23735',GETDATE(),0)
END
GO

/********************Purpose: PIR 24183
*********************Created By: Nurul Gondane********************************
*********************Comments: Message text changed *****************/

IF  EXISTS (select * from SGS_MESSAGES where [MESSAGE_ID] = 8541)
BEGIN
update SGS_MESSAGES
SET DISPLAY_MESSAGE='New Employee request for {0} was processed with errors. This request appears below.  Please review the reason for the error by clicking on the Request ID link.{1}'
WHERE [MESSAGE_ID] = 8541
END
GO

IF  EXISTS (select * from SGS_MESSAGES where [MESSAGE_ID] = 10224)
BEGIN
update SGS_MESSAGES
SET DISPLAY_MESSAGE='Employment Change request for {0} was processed with errors. This request appears below.  Please review the reason for the error by clicking on the Request ID link.{1}'
WHERE [MESSAGE_ID] = 10224
END
GO

