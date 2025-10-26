/********************Purpose: PIR 14426 ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: New BPM process *****************/


IF EXISTS(SELECT TOP 1 * FROM SGW_BPM_PROCESS WHERE NAME = 'Create and Maintain Deferred Comp Agent')
BEGIN 
	DELETE FROM SGW_BPM_PROCESS_EVENT_XR WHERE BPM_PROCESS_EVENT_XR_ID = 346

	IF NOT EXISTS(SELECT TOP 1 * FROM SGW_BPM_PROCESS_EVENT_XR WHERE PROCESS_ID = (SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE NAME = 'Create and Maintain Deferred Comp Agent'))
	BEGIN 

	INSERT INTO SGW_BPM_PROCESS_EVENT_XR (EVENT_ID,PROCESS_ID,ACTION_ID,ACTION_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ,EVENT_REASON_ID,ACTIVITY_ID)
	VALUES(
		(SELECT BPM_EVENT_ID FROM SGW_BPM_EVENT WHERE EVENT_DESC = 'NOTICE OF APPOINTMENT OF AUTHORIZED DEFERRED COMP AGENT'),
		(SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE NAME = 'Create and Maintain Deferred Comp Agent'),
		2005,'INRE', 'PIR-14426 ', GETDATE(),'PIR-14426 ', GETDATE(),0,NULL,
		(SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE NAME = 'Maintain Def Comp Agent')
	)
	END	
END

/********************Purpose: PIR 23759 Template : Incomplete Life Insurance App (ENR-5954)******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: Group Life Maintenance (ENR-5954) *****************/

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_COR_TEMPLATES] WHERE TEMPLATE_NAME = 'ENR-5954')
INSERT INTO [dbo].[SGS_COR_TEMPLATES] ([TEMPLATE_NAME], [TEMPLATE_DESC], [TEMPLATE_GROUP_ID], [TEMPLATE_GROUP_VALUE], [ACTIVE_FLAG], [DESTINATION_ID], [DESTINATION_VALUE], 
[ASSOCIATED_FORMS], [FILTER_OBJECT_ID], [FILTER_OBJECT_FIELD], [FILTER_OBJECT_VALUE], [CONTACT_ROLE_ID], [CONTACT_ROLE_VALUE], [BATCH_FLAG], [ONLINE_FLAG], [AUTO_PRINT_FLAG], [PRINTER_NAME_ID], 
[PRINTER_NAME_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ],
IMAGE_DOC_CATEGORY_ID,IMAGE_DOC_CATEGORY_VALUE,FILENET_DOCUMENT_TYPE_ID,FILENET_DOCUMENT_TYPE_VALUE,DOCUMENT_CODE) 
VALUES ( 'ENR-5954', 'Incomplete Life Insurance App', '19', 'MMBR', 'Y', '601', NULL, 
'wfmGroupLifeMaintenance;', NULL, NULL, NULL, 
'515',NULL, 'N', 'Y', 'N', '44', NULL, 'PIR 23759', GETDATE(), 'PIR 23759', GETDATE(), '0',
603,'MEMB',604,'CORR',NULL)
GO

/********************Purpose: BPM Postproduction Issue ******************************
*********************Created By: Surendra Reddy********************************
*********************Comments: When document comes in, no matter at which actvity the process is, they need to be resumed*****************/

IF EXISTS (SELECT 1 FROM SGW_BPM_PROCESS_EVENT_XR WHERE ACTIVITY_ID IS NOT NULL)
BEGIN
	UPDATE SGW_BPM_PROCESS_EVENT_XR SET ACTIVITY_ID = NULL WHERE ACTIVITY_ID IS NOT NULL
END
GO
/********************Purpose: PIR 24561******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: New error message for auto refund and refund amount > $1000*****************/

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10458)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10458,'Auto Refund exceeds $1,000.',16,'E',NULL,NULL,'PIR 24561',GETDATE(),'PIR 24561',GETDATE(),0)
GO