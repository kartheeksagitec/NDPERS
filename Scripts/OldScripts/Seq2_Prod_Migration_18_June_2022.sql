/********************Purpose: PIR 20961 ******************************
*********************Created By: Nikhil Chavan********************************
*********************Comments: PIR 20961 *****************/
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_COR_TEMPLATES] WHERE TEMPLATE_NAME = 'PER-0958')
INSERT INTO [dbo].[SGS_COR_TEMPLATES] ([TEMPLATE_NAME], [TEMPLATE_DESC], [TEMPLATE_GROUP_ID], [TEMPLATE_GROUP_VALUE], [ACTIVE_FLAG], [DESTINATION_ID], [DESTINATION_VALUE], [ASSOCIATED_FORMS], [FILTER_OBJECT_ID], [FILTER_OBJECT_FIELD], [FILTER_OBJECT_VALUE], [CONTACT_ROLE_ID], [CONTACT_ROLE_VALUE], [BATCH_FLAG], [ONLINE_FLAG], [AUTO_PRINT_FLAG], [PRINTER_NAME_ID], [PRINTER_NAME_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], IMAGE_DOC_CATEGORY_ID, IMAGE_DOC_CATEGORY_VALUE, FILENET_DOCUMENT_TYPE_ID, FILENET_DOCUMENT_TYPE_VALUE, DOCUMENT_CODE) 
VALUES ('PER-0958', 'Person Envelope - 6x9 or #10', '19', 'MMBR', 'Y', '601', 'PURG', 'wfmPersonMaintenance;', NULL, NULL, NULL, '515',NULL, 'N', 'Y', 'N', '44', NULL, 'PIR 20961', GETDATE(), 'PIR 20961', GETDATE(), '0', 603, NULL, 604, NULL, '1522')
GO

/********************Purpose: PIR 24847 – Email notifications for Urgent ESS messages ******************************
*********************Created By: Nurul********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 1)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} correspondence is available for you to download.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 2)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} elected to waive participation in {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 3)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Employment Change request for {0} was processed with errors. This request appears below.  Please review the reason for the error by clicking on the Request ID link.{1}', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 4)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Employment changes for {0}, PERSLink ID {1}, are successfully posted.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 5)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Employment for {0}, PERSLink ID {1}, has been successfully posted.Please have employee enroll in their benefit plans through MSS.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 6)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Error - File did not create. Please contact NDPERS regarding the File {0}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 7)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'File {0} processed with errors. <br />Please check Processed Files for further details.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 8)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Group Life insurance age change for {0}. These changes will appear on your PERSLink Employer Self Service (ESS) Benefit Enrollment Reports when you generate the report.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 9)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'New Employee request for {0} was processed with errors. This request appears below.  Please review the reason for the error by clicking on the Request ID link.{1}', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 10)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'New Enrollments/Change have occurred since you last generated your Benefit Enrollment/ Change Report.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 11)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Payment for {0} (Report ID - {1}) required before the report can be posted.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 12)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Payment for RETIREMENT (Report ID - {0}) is required.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 13)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'The {0} Payroll Report ID {1} has been successfully posted.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 14)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Welcome {0} correspondence is available for you to download.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 15)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Your Contact Ticket #{0} has been reviewed by NDPERS staff. Please view the Contact Ticket by following the link.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 16)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'New Employee request for {0} was rejected.  Please refer to your Pending Requests to review the reason for the error, then submit a new request with any necessary changes, if needed.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 17)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Employment Change Request for {0} was rejected.  Please refer to your Pending Requests to review the reason for the error, then submit a new request with any necessary changes, if needed.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 18)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'You have successfully registered for {0} scheduled for {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 19)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} enrolled in Deferred Compensation plan and added provider details as follows -> Provider Name : {2}, Pay Period Amount : {3:C}, Effective Start Date : {4}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 20)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} enrolled in Deferred Compensation plan and end dated the provider -> Provider Name : {2}, Pay Period Amount : {3:C}, Effective End Date : {4}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 21)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} elected to participate in Flex Comp plan.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 22)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Amount to be deducted for Dependent Care Reimbursement Account is {0:C} effective {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 23)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Amount to be deducted for Medical Spending Reimbursement Account is {0:C} effective {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 24)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'The following premiums have been selected for pre tax deductions: {0}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 25)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} ended participation in the {2} plan effective {3}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 26)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} elected to participate in the {2} insurance plan for {3} coverage with premium due of {4:C} effective {5}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 27)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} elected to participate in the Life Insurance plan.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 28)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( 'Basic Premium is {0:C} and Total Supplemental Premium is {1:C} effective {2}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 29)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} elected to participate in the LTC Insurance plan. Total monthly premium is {2:C} effective {3}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 30)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} enrolled in {2} effective {3}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 31)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} {1} elected to waive participation in {2}.', 3501, 'LOW', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 32)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} elected to waive the participation in the Main plan effective {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 33)
BEGIN
INSERT INTO SGT_WSS_DASHBOARD_MESSAGES ( [MESSAGE_TEXT], [PRIORITY_ID], [PRIORITY_VALUE], [AUDIENCE_ID], [AUDIENCE_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES( '{0} elected to waive the participation in the DC plan effective {1}.', 3501, 'HIGH', 3502,'EMPL', 'PIR 24847', GETDATE(), 'PIR 24847', GETDATE(), 0)
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


-------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE CODE_ID =  3501 AND CODE_VALUE = 'URGE')
BEGIN
	INSERT INTO [dbo].[SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], [DATA3], [COMMENTS], [START_DATE], [END_DATE],
 [CODE_VALUE_ORDER], [LEGACY_CODE_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
 VALUES (3501, N'URGE', N'Urgent', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'PIR 24847', GETDATE(),'PIR 24847', GETDATE(), 0)
END
GO
--------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE [SETTING_NAME] ='ESSRetrieveMailFrom')
BEGIN
INSERT INTO SGS_SYSTEM_SETTINGS ( [SETTING_NAME], [SETTING_TYPE], [SETTING_VALUE], [ENCRYPTED_FLAG], [REFRESHABLE])
VALUES('ESSRetrieveMailFrom','string','ndpersmss@nd.gov','N',NULL )
END
GO
-----------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE [SETTING_NAME] ='ESSEmailNotificationMsg')
BEGIN
INSERT INTO SGS_SYSTEM_SETTINGS ( [SETTING_NAME], [SETTING_TYPE], [SETTING_VALUE], [ENCRYPTED_FLAG], [REFRESHABLE])
VALUES('ESSEmailNotificationMsg','string','<p>{0},</p>   <p>You have a new message on your NDPERS online account. Please log in to your Employer Self Service (ESS) and view your most recent update under the <i>Alerts & Messages</i> section.</p>  <p><a href="https://perslink.nd.gov/perslinkess/wfmloginEE.aspx">View my NDPERS online account>></a></p>  <p><a href ="mailto:ndpers-info@nd.gov" >Contact us</a> or call us at 701-328-3900 if you have questions regarding your NDPERS benefits. </p>','N',NULL )
END
GO
--------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE [SETTING_NAME] ='ESSEmailNotificationSubject')
BEGIN
INSERT INTO SGS_SYSTEM_SETTINGS ( [SETTING_NAME], [SETTING_TYPE], [SETTING_VALUE], [ENCRYPTED_FLAG], [REFRESHABLE])
VALUES('ESSEmailNotificationSubject','string','Your NDPERS online account','N',NULL )
END
GO
-----------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE [SETTING_NAME] ='ESSMailBodySignature')
BEGIN
INSERT INTO SGS_SYSTEM_SETTINGS ( [SETTING_NAME], [SETTING_TYPE], [SETTING_VALUE], [ENCRYPTED_FLAG], [REFRESHABLE])
VALUES('ESSMailBodySignature','string','<p>Thank you for allowing us to assist you.</p>  <p>Sincerely,</p>  <b>North Dakota Public Employees Retirement System</b>  <br>  <p style ="font:Calibri; font-size:"9px"; ">   1600 East Century Avenue | PO Box 1657<br>  Bismarck, ND 58502| Online <a href=/"https://www.ndpers.nd.gov////">https://www.ndpers.nd.gov/</a></p>  <p>Do not reply to this message. </p>  <p>This e-mail, including any attachments, may contain information that is proprietary,   privileged and/or confidential and is intended exclusively for the person(s) to whom it is addressed. Any use, disclosure, copying, retention or distribution by any person other than the intended recipient or the intended recipient’s designees is strictly prohibited.  If you are not the intended recipient or their designee, please notify the sender immediately by return e-mail and delete all copies.  This message is not intended to provide specific advice or recommendations for any individuals. NDPERS is governed by the laws and regulations set forth in the N.D.C.C. and N.D.A.C. Consult your attorney, accountant, financial or tax advisor about your individual situation. NDPERS has taken reasonable precautions to ensure no viruses are present in this email; however, the agency cannot accept responsibility for any loss or damage that may arise from the use of this email or attachments.</p>','N',NULL )
END
GO
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 6)
BEGIN
UPDATE SGT_WSS_DASHBOARD_MESSAGES SET MESSAGE_TEXT='Error - File did not create. Please contact NDPERS regarding the File {0}.' WHERE WSS_DASHBOARD_MESSAGES_ID = 6
END
GO

IF EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 12)
BEGIN
UPDATE SGT_WSS_DASHBOARD_MESSAGES SET MESSAGE_TEXT='Payment for RETIREMENT (Report ID - {0}) is required.' WHERE WSS_DASHBOARD_MESSAGES_ID = 12
END
GO

IF EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 31)
BEGIN
UPDATE SGT_WSS_DASHBOARD_MESSAGES SET PRIORITY_VALUE='LOW' WHERE WSS_DASHBOARD_MESSAGES_ID = 31
END
GO


IF EXISTS (SELECT 1 FROM SGT_WSS_DASHBOARD_MESSAGES WHERE WSS_DASHBOARD_MESSAGES_ID = 9)
BEGIN
UPDATE SGT_WSS_DASHBOARD_MESSAGES SET MESSAGE_TEXT='New Employee request for {0} was processed with errors. This request appears below.  Please review the reason for the error by clicking on the Request ID link.{1}' WHERE WSS_DASHBOARD_MESSAGES_ID = 9
END
GO

