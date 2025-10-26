/*****************Purpose: Conversion ************************************
******************Created By: Aarti***************************************
******************Comments: Insert ESS application name, app name should 
						match with ESS web.config application name*******/
IF NOT EXISTS (SELECT 1 FROM SGS_ACT_LOG_APP WHERE [APP_NAME] = 'PERSLinkESS' )
INSERT INTO [dbo].[SGS_ACT_LOG_APP]
           ([APP_NAME]
           ,[ACTION_FLAG]
           ,[ACTION_STEP_FLAG]
           ,[QUERY_FLAG]
           ,[RULE_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ]
           ,[BROWSER_TIME_FLAG]
           ,[ACTION_STEP_SIZE_FLAG]
           ,[validation_rule_flag])
     VALUES
           ('PERSLinkESS'
           ,'Y'
           ,'Y'
           ,'Y'
           ,'Y'
           ,'SYSTEM'
           ,SYSDATETIME()
           ,'SYSTEM'
           ,SYSDATETIME()
           ,0
           ,'Y'
           ,'Y'
           ,'Y')
GO

/********************Purpose: Neotrack PIR ID : 15941********************
*********************Created By: Aarti***********************************
*********************Comments: In case of webform, All first item text in 
								dropdown used to be stored as default 
								value now removing all values which are 
								not applicable in MVVM ****************/

IF EXISTS(SELECT 1 FROM SGS_USER_DEFAULTS WHERE DEFAULT_VALUE IN ('ALL', 'A', '0','%')) 
DELETE FROM SGS_USER_DEFAULTS WHERE DEFAULT_VALUE IN ('ALL', 'A', '0','%')
GO

/********************Purpose: Adhoc Finding******************************
*********************Created By: Surendra********************************
*********************Comments: No need for these keys in system settings
							   anymore, tooltip messages for buttons 
							   handled in F/W***************************/
IF EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE SETTING_NAME IN 
			(SELECT MESSAGE_KEY FROM SGS_FWK_MESSAGES 
				WHERE CATEGORY_VALUE = 'TLTP'))
	DELETE FROM SGS_SYSTEM_SETTINGS 
	WHERE SETTING_NAME IN (SELECT MESSAGE_KEY FROM SGS_FWK_MESSAGES WHERE CATEGORY_VALUE = 'TLTP') 
GO
/********************Purpose: 6.0.10.0.F******************************
*********************Created By: Surendra********************************
*********************Comments: Online queries should not wait indefintely 
							 for execution of the query*****************/
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE SETTING_NAME = 'OnlineCommandTimeOut')
INSERT INTO dbo.SGS_SYSTEM_SETTINGS (SETTING_NAME, SETTING_TYPE, SETTING_VALUE, ENCRYPTED_FLAG)
VALUES ('OnlineCommandTimeOut', 'int', '1800', NULL)
GO 

/********************Purpose: 6.0.10.0.I******************************
*********************Created By: Dinesh********************************
*********************Comments: In view only mode other buttons should not visible to user *****************/
IF NOT EXISTS (SELECT 1 FROM [dbo].SGS_SYSTEM_SETTINGS WHERE SETTING_NAME = 'DisplayEditInReadOnly')
BEGIN
INSERT [dbo].[SGS_SYSTEM_SETTINGS] ([SETTING_NAME], [SETTING_TYPE], [SETTING_VALUE], [ENCRYPTED_FLAG]) 
VALUES ('DisplayEditInReadOnly', 'bool', 'false', NULL);
 END
GO 

/********************Purpose: Neotrack PIR 17278******************************
*********************Created By: Surendra********************************
*********************Comments: Since no numbers are allowed in the file name, we had to change the file name *****************/
IF EXISTS (SELECT 1 FROM SGS_COR_TEMPLATES where ASSOCIATED_FORMS <> 'wfmonezeronineninerMaintenance;' AND TEMPLATE_ID = 461)
BEGIN
UPDATE SGS_COR_TEMPLATES SET ASSOCIATED_FORMS = 'wfmonezeronineninerMaintenance;' WHERE TEMPLATE_ID = 461
 END
GO 

/********************Purpose: Neotrack PIR 17259******************************
*********************Created By: Surendra********************************
*********************Comments: F/W recommendation, no changes to save message in MVVM *****************/
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10400)
INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10400, N'Purchase Contract ID Is Required.', 16, N'E', NULL, NULL, N'FWUpgrade', GETDATE(), N'FWUpgrade', GETDATE(), 0)
GO
/********************Purpose: ESS MSS Split******************************
*********************Created By: Aarti********************************
*********************Comments: Neogrid Features provided for ESS as per existing application *****************/
IF EXISTS (Select * from SGS_USER_PREFERENCES where APPLICATION_NAME = 'PERSLinkWSS' AND USER_NAME = 'System')
BEGIN
UPDATE SGS_USER_PREFERENCES set APPLICATION_NAME = 'PERSLinkESS' where APPLICATION_NAME = 'PERSLinkWSS'
END
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID: Query used by NeoFlowService ********************************
*********************Comments: The impact may not be noticeable, but this index helps in creating process instance faster 
********************************(Confirmed with Maik we need this, consider updating statistics on a weekly basis) *****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_WORKFLOW_REQUEST_STATUS_VALUE' AND object_id = OBJECT_ID('[dbo].[SGW_WORKFLOW_REQUEST]'))
DROP INDEX [IX_WORKFLOW_REQUEST_STATUS_VALUE] ON [dbo].[SGW_WORKFLOW_REQUEST]
GO

CREATE NONCLUSTERED INDEX [IX_WORKFLOW_REQUEST_STATUS_VALUE] ON 
[dbo].[SGW_WORKFLOW_REQUEST] ([STATUS_VALUE]) 
INCLUDE ([WORKFLOW_REQUEST_ID],[DOCUMENT_CODE], [PROCESS_ID], [REFERENCE_ID], [FILENET_DOCUMENT_TYPE], [IMAGE_DOC_CATEGORY], [PERSON_ID], [ORG_CODE], [PROCESS_INSTANCE_ID],
[STATUS_ID], [SOURCE_ID], [SOURCE_VALUE], [INITIATED_DATE], [CONTACT_TICKET_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ],
[ADDITIONAL_PARAMETER1])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID: entEmployerPayrollHeader.rptDeferredCompPayrollContributionReport********************************
*********************Comments: Before index - 12 minutes and 52 seconds, Post index: 14 seconds (confirm with Maik) *****************/
update statistics SGT_EMPLOYER_PAYROLL_HEADER with fullscan

update statistics SGT_EMPLOYER_PAYROLL_DETAIL with fullscan --this will take long time (TOOK 4 AND HALF HOURS IN DEV REGION).
 
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_EMPLOYER_PAYROLL_HEADER_ID' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]'))
DROP INDEX [IX_EMPLOYER_PAYROLL_HEADER_ID] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]
GO

CREATE NONCLUSTERED INDEX [IX_EMPLOYER_PAYROLL_HEADER_ID] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]
(
       [EMPLOYER_PAYROLL_HEADER_ID] ASC,
       [POSTED_DATE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF, FILLFACTOR = 75) ON [PRIMARY]
GO 
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  SGT_COMMENTS********************************
*********************Comments: The below indexes will definitely help reduce the Payroll headers and details loading time*****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_EMPLOYER_PAYROLL_HEADER_ID_HEADER_ID' AND object_id = OBJECT_ID('[dbo].[SGT_COMMENTS]'))
DROP INDEX [IX_EMPLOYER_PAYROLL_HEADER_ID_HEADER_ID] ON [dbo].[SGT_COMMENTS]
GO

CREATE NONCLUSTERED INDEX [IX_EMPLOYER_PAYROLL_HEADER_ID_HEADER_ID] ON [dbo].[SGT_COMMENTS]
(
	[EMPLOYER_PAYROLL_HEADER_ID] ASC
	

) INCLUDE ([COMMENT_ID], [EMPLOYER_PAYROLL_DETAIL_ID], [COMMENTS], [CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF, FILLFACTOR = 75) ON [PRIMARY]
GO



IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_EMPLOYER_PAYROLL_DETAIL_ID' AND object_id = OBJECT_ID('[dbo].[SGT_COMMENTS]'))
DROP INDEX [IX_EMPLOYER_PAYROLL_DETAIL_ID] ON [dbo].[SGT_COMMENTS]
GO

CREATE NONCLUSTERED INDEX [IX_EMPLOYER_PAYROLL_DETAIL_ID] ON [dbo].[SGT_COMMENTS]
(
	[EMPLOYER_PAYROLL_DETAIL_ID] ASC
) INCLUDE ([COMMENT_ID], [EMPLOYER_PAYROLL_HEADER_ID], [COMMENTS], [CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF, FILLFACTOR = 75) ON [PRIMARY]
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entWssMemberRecordRequest.LoadMemberRecordRequest********************************
*********************Comments: Before index - 6 seconds without significant load on the server, Post index: 2 seconds
-----------------------------helps to load member record requests for an organization*****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_WSS_PERSON_EMPLOYMENT_ORGID_REQUEST_ID' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_PERSON_EMPLOYMENT]'))
DROP INDEX [IX_WSS_PERSON_EMPLOYMENT_ORGID_REQUEST_ID] ON [dbo].[SGT_WSS_PERSON_EMPLOYMENT]
GO

CREATE NONCLUSTERED INDEX [IX_WSS_PERSON_EMPLOYMENT_ORGID_REQUEST_ID]
ON [dbo].[SGT_WSS_PERSON_EMPLOYMENT] ([ORG_ID])
INCLUDE ([MEMBER_RECORD_REQUEST_ID])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entEmployerPayrollDetail.GetContributionFileForAudit********************************
*********************Comments: Before index - 2 minute and 1 seconds, Post index: 35 seconds*****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_EMP_PAY_DETAIL_PLAN_ID_POSTED_DATE' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]'))
DROP INDEX [IX_EMP_PAY_DETAIL_PLAN_ID_POSTED_DATE] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]
GO

CREATE NONCLUSTERED INDEX [IX_EMP_PAY_DETAIL_PLAN_ID_POSTED_DATE]
ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL] ([PLAN_ID],[POSTED_DATE])
INCLUDE ([EMPLOYER_PAYROLL_DETAIL_ID],[EMPLOYER_PAYROLL_HEADER_ID],[PERSON_ID],[FIRST_NAME],[LAST_NAME],[RECORD_TYPE_VALUE],[CONTRIBUTION_AMOUNT1],
		[PROVIDER_ORG_CODE_ID1],[CONTRIBUTION_AMOUNT2],[PROVIDER_ORG_CODE_ID2],[CONTRIBUTION_AMOUNT3],[PROVIDER_ORG_CODE_ID3],[CONTRIBUTION_AMOUNT4],
		[PROVIDER_ORG_CODE_ID4],[CONTRIBUTION_AMOUNT5],[PROVIDER_ORG_CODE_ID5],[CONTRIBUTION_AMOUNT6],[PROVIDER_ORG_CODE_ID6],[CONTRIBUTION_AMOUNT7],
		[PROVIDER_ORG_CODE_ID7],[ELIGIBLE_WAGES],[EE_CONTRIBUTION_CALCULATED],[EE_PRE_TAX_CALCULATED],[EE_EMPLOYER_PICKUP_CALCULATED],[ER_CONTRIBUTION_CALCULATED],
		[RHIC_ER_CONTRIBUTION_CALCULATED],[RHIC_EE_CONTRIBUTION_CALCULATED])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entWssPersonAccountEnrollmentRequest.GetRequestIDForAnnualEnrollment********************************
*********************Comments: this index helps to load wfmAnnualEnrollmentBenefitPlansMaintenance page faster*****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_PAER_PERSON_ID_PLAN_ID_DATE_OF_CHANGE_STATUS_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST]'))
DROP INDEX [IX_PAER_PERSON_ID_PLAN_ID_DATE_OF_CHANGE_STATUS_VALUE] ON [dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST]
GO

CREATE NONCLUSTERED INDEX [IX_PAER_PERSON_ID_PLAN_ID_DATE_OF_CHANGE_STATUS_VALUE]
ON [dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST] ([PERSON_ID],[PLAN_ID],[DATE_OF_CHANGE],[STATUS_VALUE])
GO
 /********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  cdoWssMessageHeader.UpdateReportLink********************************
*********************Comments: Before index - 4 seconds, Post index: 0 seconds*****************/

---Need to check from here.

IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_WMD_ORG_ID_COR_LINK' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_MESSAGE_DETAIL]'))
DROP INDEX [IX_WMD_ORG_ID_COR_LINK] ON [dbo].[SGT_WSS_MESSAGE_DETAIL]
GO

CREATE NONCLUSTERED INDEX [IX_WMD_ORG_ID_COR_LINK]
ON [dbo].[SGT_WSS_MESSAGE_DETAIL] ([PERSON_ID],[ORG_ID],[CORRESPONDENCE_LINK],[CONTACT_ID],[CLEAR_MESSAGE_FLAG])
INCLUDE ([WSS_MESSAGE_DETAIL_ID],[WSS_MESSAGE_ID], [REPORT_CREATED_DATE])
GO

IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_SGT_WSS_MESSAGE_DETAIL_ORG_ID_CONTACT_ID_CLEAR_MESSAGE_FLAG' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_MESSAGE_DETAIL]'))
DROP INDEX [IX_SGT_WSS_MESSAGE_DETAIL_ORG_ID_CONTACT_ID_CLEAR_MESSAGE_FLAG] ON [dbo].[SGT_WSS_MESSAGE_DETAIL]
GO


CREATE INDEX [IX_SGT_WSS_MESSAGE_DETAIL_ORG_ID_CONTACT_ID_CLEAR_MESSAGE_FLAG] ON 
[dbo].[SGT_WSS_MESSAGE_DETAIL] ([ORG_ID], [CONTACT_ID], [CLEAR_MESSAGE_FLAG]) 
INCLUDE ([WSS_MESSAGE_DETAIL_ID], [WSS_MESSAGE_ID], [PERSON_ID], [WEB_LINK], [CORRESPONDENCE_LINK], [TRACKING_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], 
[MODIFIED_DATE], [UPDATE_SEQ], [TEMPLATE_NAME], [REPORT_CREATED_DATE], [MSS_EMAIL_SENT_FLAG]) WITH (FILLFACTOR = 90, ONLINE= OFF, SORT_IN_TEMPDB = OFF)
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  cdoEmployerPayrollHeader.UpdateRetrInsrEmpHeaderNORFromARBatch********************************
*********************Comments: Before index - 10 seconds, in production taking longer, Post index: 0 seconds*****************/
IF EXISTS (SELECT 1 FROM sys.indexes 
WHERE name='IX_EPH_STATUS_BAL_HEADER_TYPE_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_HEADER]'))
DROP INDEX [IX_EPH_STATUS_BAL_HEADER_TYPE_VALUE] ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER]
GO

CREATE NONCLUSTERED INDEX [IX_EPH_STATUS_BAL_HEADER_TYPE_VALUE]
ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER] ([STATUS_VALUE],[BALANCING_STATUS_VALUE],[HEADER_TYPE_VALUE])
INCLUDE ([EMPLOYER_PAYROLL_HEADER_ID],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entEmployerPayrollHeader.GetErrorOnESS********************************
*********************Comments: Could find this query taking a long time in audit log queries*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_EPDE_DETAIL_ID_MESSAGE_ID' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_DETAIL_ERROR]'))
DROP INDEX [IX_EPDE_DETAIL_ID_MESSAGE_ID] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL_ERROR]
GO
CREATE NONCLUSTERED INDEX [IX_EPDE_DETAIL_ID_MESSAGE_ID]
ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL_ERROR] ([EMPLOYER_PAYROLL_DETAIL_ID], [MESSAGE_ID])
GO

IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_EPDH_HEADER_ID_MESSAGE_ID' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_HEADER_ERROR]'))
DROP INDEX [IX_EPDH_HEADER_ID_MESSAGE_ID] ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER_ERROR]
GO
CREATE NONCLUSTERED INDEX [IX_EPDH_HEADER_ID_MESSAGE_ID]
ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER_ERROR] ([EMPLOYER_PAYROLL_HEADER_ID], [MESSAGE_ID])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  cdoWssPersonAccountEnrollmentRequest.UpdateBenefitEnrollmentFlag********************************
*********************Comments: Could find this query taking a long time in audit log queries*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_ENROLLMENT_DATA_IS_REPORT_GENERATED' AND object_id = OBJECT_ID('[dbo].[SGT_ENROLLMENT_DATA]'))
DROP INDEX IX_EPDE_DETAIL_ID_MESSAGE_ID ON [dbo].[SGT_ENROLLMENT_DATA]
GO
CREATE NONCLUSTERED INDEX [IX_ENROLLMENT_DATA_IS_REPORT_GENERATED]
ON [dbo].[SGT_ENROLLMENT_DATA] ([EMPLOYER_ORG_ID],[PLAN_STATUS_VALUE],[PERSON_ACCOUNT_ID], [EMPLOYMENT_TYPE_VALUE], [SOURCE_ID], [PEOPLESOFT_ID])
INCLUDE ([ENROLLMENT_DATA_ID],[PLAN_ID],[NDPERS_MEMBER_ID],[START_DATE],[END_DATE],[IS_BENEFIT_ENROLLMENT_REPORT_GENERATED], [PEOPLESOFT_FILE_SENT_FLAG])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entEmployerPayrollDetail.GetCountOfRecordsPerPersonOrgPayDateReportForRetAndIns********************************
*********************Comments: Could find this query taking a long time in audit log queries*****************/
/****** Object:  Index [IX_PAY_PERIOD_DATE_ANDMORE]    Script Date: 7/29/2020 12:11:43 PM ******/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_EPD_HEADER_ID_AND_MORE' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]'))
DROP INDEX [IX_EPD_HEADER_ID_AND_MORE] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]
GO

/****** Object:  Index [IX_PAY_PERIOD_DATE_ANDMORE]    Script Date: 7/29/2020 12:11:43 PM ******/
CREATE NONCLUSTERED INDEX [IX_EPD_HEADER_ID_AND_MORE] ON [dbo].[SGT_EMPLOYER_PAYROLL_DETAIL]
(
	[EMPLOYER_PAYROLL_HEADER_ID] ASC,
	[PAY_PERIOD_DATE] ASC,
	[PLAN_ID] ASC,
	[RECORD_TYPE_VALUE] ASC,
	[STATUS_VALUE] ASC
) INCLUDE (PERSON_ID) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  SGT_EMPLOYMENT_CHANGE_REQUEST********************************
*********************Comments: Do not have any index other than on primary key, LOOKING AT THE QUERIES, THIS TABLE NEEDS THIS ONE*****************/
/****** Object:  Index [IX_PAY_PERIOD_DATE_ANDMORE]    Script Date: 7/29/2020 12:11:43 PM ******/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_WECR_PERSON_ID_ORG_ID_STATUS_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_EMPLOYMENT_CHANGE_REQUEST]'))
DROP INDEX [IX_WECR_PERSON_ID_ORG_ID_STATUS_VALUE] ON [dbo].[SGT_WSS_EMPLOYMENT_CHANGE_REQUEST]
GO

CREATE NONCLUSTERED INDEX [IX_WECR_PERSON_ID_ORG_ID_STATUS_VALUE] ON [dbo].[SGT_WSS_EMPLOYMENT_CHANGE_REQUEST]
(
	[PERSON_ID] ASC, 
	[ORG_ID] ASC, 
	[STATUS_VALUE] ASC,
	[CHANGE_TYPE_VALUE] ASC

) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION********************************
*********************Comments: Main table for calculation and pension plan maintenance as well*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_EFFECTIVE_DATE' AND object_id = OBJECT_ID('[dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]'))
DROP INDEX [IX_EFFECTIVE_DATE] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]
GO

CREATE NONCLUSTERED INDEX [IX_EFFECTIVE_DATE]
ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION] ([EFFECTIVE_DATE])
INCLUDE ([PERSON_ACCOUNT_ID],[POST_TAX_EE_AMOUNT],[PRE_TAX_ER_AMOUNT],[PRE_TAX_EE_AMOUNT],[EE_ER_PICKUP_AMOUNT],[ER_VESTED_AMOUNT],[INTEREST_AMOUNT])
GO

IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_TRANSACTION_TYPE_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]'))
DROP INDEX [IX_TRANSACTION_TYPE_VALUE] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]
GO


/****** Object:  Index [IX_TRANSACTION_TYPE_VALUE]    Script Date: 7/29/2020 4:34:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_TRANSACTION_TYPE_VALUE] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]
(
	[TRANSACTION_TYPE_VALUE] ASC,
	[TRANSACTION_DATE] ASC
) INCLUDE ([PERSON_EMPLOYMENT_DTL_ID],[ER_VESTED_AMOUNT]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SUB_SYSTEM_VALUE]    Script Date: 7/29/2020 4:34:16 PM ******/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SUB_SYSTEM_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]'))
DROP INDEX [IX_SUB_SYSTEM_VALUE] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]
GO

/****** Object:  Index [IX_SUB_SYSTEM_VALUE]    Script Date: 7/29/2020 4:34:16 PM ******/
CREATE NONCLUSTERED INDEX [IX_SUB_SYSTEM_VALUE] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]
(
	[SUBSYSTEM_VALUE] ASC,
	[TRANSACTION_DATE]
) INCLUDE ([PERSON_ACCOUNT_ID],[EE_RHIC_AMOUNT],[INTEREST_AMOUNT],[EMPLOYER_RHIC_INTEREST]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION********************************
*********************Comments: Will help faster loading of wfmPensionPlanMaintenance*****************/
/****** Object:  Index [IX_PAY_PERIOD_DATE_ANDMORE]    Script Date: 7/29/2020 12:11:43 PM ******/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SERVICE_PURCHASE_DETAIL_SERVICE_CREDIT_TYPE' AND object_id = OBJECT_ID('[dbo].[SGT_SERVICE_PURCHASE_DETAIL_CONSOLIDATED]'))
DROP INDEX [IX_SERVICE_PURCHASE_DETAIL_SERVICE_CREDIT_TYPE] ON [dbo].[SGT_SERVICE_PURCHASE_DETAIL_CONSOLIDATED]
GO

CREATE NONCLUSTERED INDEX [IX_SERVICE_PURCHASE_DETAIL_SERVICE_CREDIT_TYPE]
ON [dbo].[SGT_SERVICE_PURCHASE_DETAIL_CONSOLIDATED] ([SERVICE_CREDIT_TYPE_VALUE])
INCLUDE ([SERVICE_PURCHASE_DETAIL_ID])
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entJournalDetail.GetAmountGroupByFundValue********************************
*********************Comments: Will help faster loading of the form*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_FUND_VALUE_DEBIT_CREDIT_AMOUNT' AND object_id = OBJECT_ID('[dbo].[SGT_JOURNAL_DETAIL]'))
DROP INDEX [IX_FUND_VALUE_DEBIT_CREDIT_AMOUNT] ON [dbo].[SGT_JOURNAL_DETAIL]
GO

CREATE INDEX [IX_FUND_VALUE_DEBIT_CREDIT_AMOUNT] ON [dbo].[SGT_JOURNAL_DETAIL]
(
	[JOURNAL_HEADER_ID]

) INCLUDE ([FUND_VALUE], [DEBIT_AMOUNT], [CREDIT_AMOUNT])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitApplication.IsTermDateRetContributionPosted********************************
*********************Comments: Will help Ben app verification process to be faster*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_PARC_PERSON_ACCOUNT_ID_EFFECTIVE_DATE_PAYPERIOD_MONTH' AND object_id = OBJECT_ID('[dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION]'))
DROP INDEX [IX_PARC_PERSON_ACCOUNT_ID_EFFECTIVE_DATE_PAYPERIOD_MONTH] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION] 
GO

	CREATE INDEX [IX_PARC_PERSON_ACCOUNT_ID_EFFECTIVE_DATE_PAYPERIOD_MONTH] ON [dbo].[SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION] 
	(
		[PERSON_ACCOUNT_ID],
		[SUBSYSTEM_VALUE],
		[EFFECTIVE_DATE],
		[PAY_PERIOD_YEAR],
		[PAY_PERIOD_MONTH]
	
	) INCLUDE ([PENSION_SERVICE_CREDIT],[VESTED_SERVICE_CREDIT])
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitApplication.LoadBenefitApplicationPersonAccount********************************
*********************Comments: Gets called while validating new application*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SPS_SCHEDULE_TYPE_STATUS' AND object_id = OBJECT_ID('[dbo].[SGT_PAYMENT_SCHEDULE]'))
DROP INDEX [IX_SPS_SCHEDULE_TYPE_STATUS] ON [dbo].[SGT_PAYMENT_SCHEDULE] 
GO

CREATE NONCLUSTERED INDEX [IX_SPS_SCHEDULE_TYPE_STATUS]
ON [dbo].[SGT_PAYMENT_SCHEDULE] ([SCHEDULE_TYPE_VALUE],[STATUS_VALUE])
INCLUDE ([PAYMENT_DATE])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitApplication.LoadRefundBenefitCalculation********************************
*********************Comments: Used eight times in the code*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SBC_APPLICATION_ID' AND object_id = OBJECT_ID('[dbo].[SGT_BENEFIT_CALCULATION]'))
DROP INDEX [IX_SBC_APPLICATION_ID] ON [dbo].[SGT_BENEFIT_CALCULATION]
GO


CREATE NONCLUSTERED INDEX [IX_SBC_APPLICATION_ID]
ON [dbo].[SGT_BENEFIT_CALCULATION] ([BENEFIT_APPLICATION_ID])
INCLUDE ([BENEFIT_CALCULATION_ID],[PERSON_ID],[PLAN_ID],[CALCULATION_TYPE_ID],[CALCULATION_TYPE_VALUE],[BENEFIT_ACCOUNT_TYPE_ID],[BENEFIT_ACCOUNT_TYPE_VALUE],
[BENEFIT_ACCOUNT_SUB_TYPE_ID],[BENEFIT_ACCOUNT_SUB_TYPE_VALUE],[PLSO_REQUESTED_FLAG],[BENEFIT_OPTION_ID],[BENEFIT_OPTION_VALUE],[UNIFORM_INCOME_OR_SSLI_FLAG],
[SSLI_OR_UNIFORM_INCOME_COMMENCEMENT_AGE],[ESTIMATED_SSLI_BENEFIT_AMOUNT],[REDUCED_BENEFIT_FLAG],[REDUCED_BENEFIT_OPTION_AMOUNT],[TERMINATION_DATE],[NORMAL_RETIREMENT_DATE],
[RETIREMENT_DATE],[DATE_OF_DEATH],[COMBINED_DUAL_FAS_FLAG],[RHIC_OPTION_ID],[RHIC_OPTION_VALUE],[UNREDUCED_RHIC_AMOUNT],[RHIC_EARLY_REDUCTION_FACTOR],[ADJUSTED_PSC],
[ADJUSTED_TVSC],[COMMENTS],[JS_RHIC_AMOUNT],[PAID_UP_ANNUITY_AMOUNT],[COMPUTED_FINAL_AVERAGE_SALARY],[OVERRIDDEN_FINAL_AVERAGE_SALARY],[UNREDUCED_BENEFIT_AMOUNT],[CREDITED_PSC],
[PROJECTED_PSC],[CREDITED_VSC],[PROJECTED_VSC],[INDEXED_FINAL_AVERAGE_SALARY],[PERCENTAGE_SALARY_INCREASE],[SALARY_MONTH_INCREASE],[DNRO_MISSED_MONTHS],[ADHOC_OR_COLA_AMOUNT],
[DNRO_TOTAL_MISSED_AMOUNT],[DNRO_FACTOR],[DNRO_MONTHLY_INCREASE],[EARLY_REDUCED_MONTHS],[EARLY_REDUCTION_FACTOR],[PLSO_LUMPSUM_AMOUNT],[PLSO_FACTOR],[PLSO_REDUCTION_AMOUNT],
[MINIMUM_GUARENTEE_AMOUNT],[MINIMUM_GUARENTEE_AMOUNT_TAXABLE_AMOUNT],[MINIMUM_GUARENTEE_AMOUNT_NON_TAXABLE_AMOUNT],[TAXABLE_AMOUNT],[NON_TAXABLE_AMOUNT],
[JS_RESIDUAL_MG_AMOUNT],[STATUS_ID],[STATUS_VALUE],[ACTION_STATUS_ID],[ACTION_STATUS_VALUE],[APPROVED_BY],[SUPPRESS_WARNINGS_FLAG],[SUPPRESS_WARNINGS_BY],
[SUPPRESS_WARNINGS_DATE],[EARLY_MONTHLY_DECREASE],[PLSO_EXCLUSION_RATIO],[SSLI_UNIFORM_INCOME_FACTOR],[QDRO_AMOUNT],[TAXABLE_QDRO_AMOUNT],[NON_TAXABLE_QDRO_AMOUNT],
[DISABILITY_PAYEE_ACCOUNT_ID],[IS_RULE_OR_AGE_CONVERSION],[FINAL_MONTHLY_BENEFIT],[RMD_AMOUNT],[ACTUARIAL_BENEFIT_REDUCTION],[RTW_REFUND_ELECTION_ID],[RTW_REFUND_ELECTION_VALUE],
[IS_RTW_LESS_THAN_2YEARS_FLAG],[PRE_RTW_PAYEE_ACCOUNT_ID],[ACTUARIALLY_ADJUSTED_MONTHLY_SINGLE_LIFE_BENEFIT],[IS_RULE_APPLIED_FLAG],[RHIC_FACTOR_AMOUNT],
[ORIGINATING_PAYEE_ACCOUNT_ID],[POST_RETIREMENT_DEATH_REASON_TYPE_ID],[POST_RETIREMENT_DEATH_REASON_TYPE_VALUE],[BENEFICIARY_PERSON_ID],[CREATED_BY],[CREATED_DATE],
[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[PARENT_BENEFIT_CALCULATION_ID],[IS_CALCULATION_VISIBLE_FLAG],[RULE_INDICATOR_ID],[RULE_INDICATOR_VALUE],[SSLI_EFFECTIVE_DATE],
[RECALCULATED_BY],[IS_CREATED_FROM_PORTAL],[NON_TAXABLE_PLSO],[FAS_TERMINATION_DATE],[IS_TENTATIVE_TFFR_TIAA_USED],[GRADUATED_BENEFIT_OPTION_ID],[GRADUATED_BENEFIT_OPTION_VALUE],
[RTW_GRADUATED_BENEFIT_FACTOR],[OVERRIDDEN_DNRO_MISSED_PAYMENT_AMOUNT],[is_dro_estimate],[TFFR_CALCULATION_METHOD_ID],[TFFR_CALCULATION_METHOD_VALUE],
[RHIC_EFFECTIVE_DATE],[FAS_2010],[FAS_2019],[FAS_2020])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitProvisionBenefitType.GetBenefitProvisionByPlan********************************
*********************Comments: Gets called in benefit calculation *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_BPBT_BENEFIT_TYPE_VALUE_EFFECTIVE_DATE' AND object_id = OBJECT_ID('[dbo].[SGT_BENEFIT_PROVISION_BENEFIT_TYPE]'))
DROP INDEX [IX_BPBT_BENEFIT_TYPE_VALUE_EFFECTIVE_DATE] ON [dbo].[SGT_BENEFIT_PROVISION_BENEFIT_TYPE] 
GO

CREATE INDEX [IX_BPBT_BENEFIT_TYPE_VALUE_EFFECTIVE_DATE] ON [dbo].[SGT_BENEFIT_PROVISION_BENEFIT_TYPE] 
(
	[BENEFIT_ACCOUNT_TYPE_VALUE],
	[EFFECTIVE_DATE],
	[BENEFIT_TIER_VALUE]
) INCLUDE (

	[BENEFIT_PROVISION_ID],[BENEFIT_ACCOUNT_TYPE_ID],[BENEFIT_FORMULA_ID],[BENEFIT_FORMULA_VALUE],[EARLY_REDUCTION_METHOD_ID],[EARLY_REDUCTION_METHOD_VALUE],[EARLY_REDUCTION_FACTOR],
	[FAS_FORMULA_ID],[FAS_FORMULA_VALUE],[FAS_NO_PERIODS],[FAS_NO_PERIODS_RANGE],[PLSO_FLAG],[PLSO_FACTOR_METHOD_ID],[PLSO_FACTOR_METHOD_VALUE],[PLSO_FACTOR],[DNRO_FLAG],
	[DNRO_FACTOR_METHOD_ID],[DNRO_FACTOR_METHOD_VALUE],	[DNRO_FACTOR],[RHIC_SERVICE_FACTOR],[BENEFIT_TIER_ID]

)
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitDnroPlsoEarlyFactor.LoadBenefitDnroPlsoEarlyFactor********************************
*********************Comments: Gets called in benefit calculation *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_BEN_DNRO_PLSO_FACTOR' AND object_id = OBJECT_ID('[dbo].[SGT_BENEFIT_DNRO_PLSO_EARLY_FACTOR]'))
DROP INDEX [IX_BEN_DNRO_PLSO_FACTOR] ON [dbo].[SGT_BENEFIT_DNRO_PLSO_EARLY_FACTOR] 
GO

CREATE NONCLUSTERED INDEX [IX_BEN_DNRO_PLSO_FACTOR]
ON [dbo].[SGT_BENEFIT_DNRO_PLSO_EARLY_FACTOR] ([TRAN_TYPE],[PLAN_ID],[MEMBER_AGE],[EFFECTIVE_DATE])
INCLUDE ([BENEFIT_DNRO_PLSO_FACTOR_ID],[BEN_AGE],[SUB_TYPE],[FACTOR],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitSSLIFactor.GetSSLIFactor********************************
*********************Comments: Gets called in benefit calculation *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_BEN_SSLI_FACTOR_PLAN_EFFECTIVE_DATE' AND object_id = OBJECT_ID('[dbo].[SGT_BENEFIT_SSLI_FACTOR]'))
DROP INDEX [IX_BEN_SSLI_FACTOR_PLAN_EFFECTIVE_DATE] ON [dbo].[SGT_BENEFIT_SSLI_FACTOR]
GO

CREATE NONCLUSTERED INDEX [IX_BEN_SSLI_FACTOR_PLAN_EFFECTIVE_DATE]
ON [dbo].[SGT_BENEFIT_SSLI_FACTOR] ([PLAN_ID],[MEMBER_AGE],[EFFECTIVE_DATE])
INCLUDE ([BENEFIT_SSLI_FACTOR_ID],[SSLI_AGE],[SSLI_FACTOR],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entBenefitOptionFactor.GetOptionsFactorByPlan********************************
*********************Comments: Gets called in benefit calculation *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SBOF_MEMBER_TYPE_PLAN_OPTION' AND object_id = OBJECT_ID('[dbo].[SGT_BENEFIT_OPTION_FACTOR]'))
DROP INDEX [IX_SBOF_MEMBER_TYPE_PLAN_OPTION] ON [dbo].[SGT_BENEFIT_OPTION_FACTOR]
GO

CREATE NONCLUSTERED INDEX [IX_SBOF_MEMBER_TYPE_PLAN_OPTION]
ON [dbo].[SGT_BENEFIT_OPTION_FACTOR] ([MEMBER_AGE],[BENEFIT_TYPE],[PLAN_ID],[BENEFIT_OPTION_VALUE],[EFFECTIVE_DATE])
INCLUDE ([BENEFIT_OPTION_FACTOR_ID],[BEN_AGE],[FACTOR],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[BENEFIT_OPTION_ID])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  SGT_COR_TRACKING********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SCT_TEMPLATE_PERSON_ID' AND object_id = OBJECT_ID('[dbo].[SGS_COR_TRACKING]'))
DROP INDEX [IX_SCT_TEMPLATE_PERSON_ID] ON [dbo].[SGS_COR_TRACKING]
GO

CREATE NONCLUSTERED INDEX [IX_SCT_TEMPLATE_PERSON_ID]
ON [dbo].[SGS_COR_TRACKING] ([TEMPLATE_ID],[PERSON_ID]) 
INCLUDE ([TRACKING_ID],[PLAN_ID],[ORG_CONTACT_ID],[COR_STATUS_ID],[COR_STATUS_VALUE],[GENERATED_DATE],[PRINT_ON_DATE],[PRINTED_DATE],[IMAGING_SERIAL_NO],[COMMENTS],[CREATED_BY],
[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[IMAGED_DATE],[ORG_ID],[CONVERTED_TO_IMAGE_FLAG],[CONTACT_ID])
GO

/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entContactTicket.LoadESSAppointments********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SCT_ORGID_STATUS_CONTACT_TYPE_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_CONTACT_TICKET]'))
DROP INDEX [IX_SCT_ORGID_STATUS_CONTACT_TYPE_VALUE] ON [dbo].[SGT_CONTACT_TICKET] 
GO

CREATE INDEX [IX_SCT_ORGID_STATUS_CONTACT_TYPE_VALUE] ON [dbo].[SGT_CONTACT_TICKET] 
(
	[ORG_ID],
	[CONTACT_TYPE_VALUE],
	[STATUS_VALUE],
	[MODIFIED_DATE],
	[IS_TICKET_CREATED_FROM_PORTAL_FLAG]
) INCLUDE 

([CONTACT_TICKET_ID],[PERSON_ID],[STATUS_ID],[CALLBACK_PHONE],[EMAIL],	[CONTACT_TYPE_ID],[EVENT_TYPE_ID],[EVENT_TYPE_VALUE],[CALLER_NAME],[CALLER_RELATIONSHIP_ID],
	[CALLER_RELATIONSHIP_VALUE],[CONTACT_METHOD_ID],[CONTACT_METHOD_VALUE],[RESPONSE_METHOD_ID],[RESPONSE_METHOD_VALUE],[ASSIGN_TO_USER_ID],[NOTES],[TICKET_TYPE_ID],[TICKET_TYPE_VALUE],
	[COPY_STATUS_FLAG],[ORIGINAL_CONTACT_TICKET_ID],[CREATED_BY],	[CREATED_DATE],	[MODIFIED_BY],[UPDATE_SEQ],[TIME_OF_DAY_ID],[TIME_OF_DAY_VALUE],[IDB_REMITTANCE_CREATED_FLAG],
	[WEB_CONTACT_ID])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entContactTicket.LoadESSContactTickets********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_SAS_CONTACT_TICKET_ID_APPOINTMENTS' AND object_id = OBJECT_ID('[dbo].[SGT_APPOINTMENT_SCHEDULE]'))
DROP INDEX [IX_SAS_CONTACT_TICKET_ID_APPOINTMENTS] ON [dbo].[SGT_APPOINTMENT_SCHEDULE] 
GO

CREATE NONCLUSTERED INDEX [IX_SAS_CONTACT_TICKET_ID_APPOINTMENTS]
ON [dbo].[SGT_APPOINTMENT_SCHEDULE] ([CONTACT_TICKET_ID])
INCLUDE ([APPOINTMENT_SCHEDULE_ID],[APPOINTMENT_NAME],[APPOINTMENT_TYPE_ID],[APPOINTMENT_TYPE_VALUE],[COUNSELOR_USER_ID],[APPOINTMENT_DATE],[START_TIME_ID],[START_TIME_VALUE],[END_TIME_ID],[END_TIME_VALUE],[APPOINTMENT_CANCEL_FLAG],[NOTES],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[MEETING_REQUEST_UID],[REQUESTED_DATE_FROM],[REQUESTED_DATE_TO],[TIME_OF_DAY_ID],[TIME_OF_DAY_VALUE])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entOrgPlan.LoadActiveEmployerOrgPlanByBenefitType********************************
*********************Comments:  Gets called from 12 different places*****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_EPH_HEADER_TYPE_VALUE_REPORT_TYPE_VALUE_STATUS_VALUE' AND object_id = OBJECT_ID('[dbo].[SGT_EMPLOYER_PAYROLL_HEADER]'))
DROP INDEX [IX_EPH_HEADER_TYPE_VALUE_REPORT_TYPE_VALUE_STATUS_VALUE] ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER] 
GO


CREATE NONCLUSTERED INDEX [IX_EPH_HEADER_TYPE_VALUE_REPORT_TYPE_VALUE_STATUS_VALUE]
ON [dbo].[SGT_EMPLOYER_PAYROLL_HEADER] ([HEADER_TYPE_VALUE],[REPORT_TYPE_VALUE],[STATUS_VALUE])
INCLUDE ([EMPLOYER_PAYROLL_HEADER_ID],[ORG_ID],[HEADER_TYPE_ID],[REPORTING_SOURCE_ID],[REPORTING_SOURCE_VALUE],[REPORT_TYPE_ID],[PAYROLL_PAID_DATE],[PAY_PERIOD_START_DATE],
[PAY_PERIOD_END_DATE],[STATUS_ID],[RECEIVED_DATE],[INTEREST_WAIVER_FLAG],[BALANCING_STATUS_ID],[BALANCING_STATUS_VALUE],[TOTAL_CONTRIBUTION_REPORTED],
[TOTAL_CONTRIBUTION_CALCULATED],[TOTAL_WAGES_REPORTED],[TOTAL_WAGES_CALCULATED],[TOTAL_INTEREST_REPORTED],[TOTAL_INTEREST_CALCULATED],
[TOTAL_DETAIL_RECORD_COUNT],[TOTAL_PURCHASE_AMOUNT],[TOTAL_PREMIUM_AMOUNT_REPORTED],[TOTAL_PURCHASE_AMOUNT_REPORTED],[COMMENTS],[SUBMITTED_DATE],
[VALIDATED_DATE],[POSTED_DATE],[CENTRAL_PAYROLL_RECORD_ID],[IGNORE_BALANCING_STATUS_FLAG],[LAST_RELOAD_RUN_DATE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],
[MODIFIED_DATE],[UPDATE_SEQ],[PAY_CHECK_DATE],[SUPPRESS_SALARY_VARIANCE_VALIDATION_FLAG],[TOTAL_WAGES_ORIGINAL],[TOTAL_CONTRIBUTION_ORIGINAL],[TOTAL_PURCHASE_AMOUNT_ORIGINAL],
[SUPPRESS_3RD_PAYROLL_FLAG])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  [SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST]********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_TARGET_PERSON_ACCOUNT_ID' AND object_id = OBJECT_ID('[dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST]'))
DROP INDEX [IX_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_TARGET_PERSON_ACCOUNT_ID] ON [dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST]
GO

CREATE INDEX [IX_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_TARGET_PERSON_ACCOUNT_ID] ON [dbo].[SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST] 
(
	TARGET_PERSON_ACCOUNT_ID
)
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entPaymentHistoryHeader.LoadVendorPaymentHistoryHeader********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_PRP_BATCH_REQUEST_EFFECTIVE_DATE' AND object_id = OBJECT_ID('[dbo].[SGT_PROVIDER_REPORT_PAYMENT]'))
DROP INDEX [IX_PRP_BATCH_REQUEST_EFFECTIVE_DATE] ON [dbo].[SGT_PROVIDER_REPORT_PAYMENT]
GO
CREATE NONCLUSTERED INDEX [IX_PRP_BATCH_REQUEST_EFFECTIVE_DATE]
ON [dbo].[SGT_PROVIDER_REPORT_PAYMENT] ([BATCH_REQUEST_ID],[EFFECTIVE_DATE])
INCLUDE ([PROVIDER_ORG_ID])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entIbsDetail.LoadACHIBSDetail********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_IBS_DETAIL_PAYMENT_MODE_DEPOSIT' AND object_id = OBJECT_ID('[dbo].[SGT_IBS_DETAIL]'))
DROP INDEX [IX_IBS_DETAIL_PAYMENT_MODE_DEPOSIT] ON [dbo].[SGT_IBS_DETAIL]
GO
CREATE NONCLUSTERED INDEX [IX_IBS_DETAIL_PAYMENT_MODE_DEPOSIT]
ON [dbo].[SGT_IBS_DETAIL] ([IBS_HEADER_ID],[MODE_OF_PAYMENT_VALUE],[DEPOSIT_ID])
INCLUDE ([IBS_DETAIL_ID],[PERSON_ID],[PLAN_ID],[BILLING_MONTH_AND_YEAR],[MODE_OF_PAYMENT_ID],[COVERAGE_CODE],[MEMBER_PREMIUM_AMOUNT],[RHIC_AMOUNT],[TOTAL_PREMIUM_AMOUNT],[BALANCE_FORWARD],[COMMENT],[GROUP_HEALTH_FEE_AMT],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[PERSON_ACCOUNT_ID],[LIFE_BASIC_PREMIUM_AMOUNT],[LIFE_SUPP_PREMIUM_AMOUNT],[LIFE_SPOUSE_SUPP_PREMIUM_AMOUNT],[LIFE_DEP_SUPP_PREMIUM_AMOUNT],[OTHR_RHIC_AMOUNT],[JS_RHIC_AMOUNT],[LTC_MEMBER_THREE_YRS_PREMIUM_AMOUNT],[LTC_MEMBER_FIVE_YRS_PREMIUM_AMOUNT],[LTC_SPOUSE_THREE_YRS_PREMIUM_AMOUNT],[LTC_SPOUSE_FIVE_YRS_PREMIUM_AMOUNT],[PROVIDER_PREMIUM_AMOUNT],[GROUP_NUMBER],[PAYMENT_ELECTION_ADJUSTMENT_ID],[PAID_PREMIUM_AMOUNT],[LIFE_BASIC_COVERAGE_AMOUNT],[LIFE_SUPP_COVERAGE_AMOUNT],[LIFE_SPOUSE_SUPP_COVERAGE_AMOUNT],[LIFE_DEP_SUPP_COVERAGE_AMOUNT],[AD_AND_D_BASIC_PREMIUM_RATE],[AD_AND_D_SUPPLEMENTAL_PREMIUM_RATE],[PROVIDER_ORG_ID],[RATE_STRUCTURE_CODE],[COVERAGE_CODE_VALUE],[BUYDOWN_AMOUNT],[MEDICARE_PART_D_AMT],[RHIC_SENT],[DETAIL_STATUS_ID],[DETAIL_STATUS_VALUE],[LIS_AMOUNT],[LEP_AMOUNT])
GO
/********************Purpose: Performance indexing******************************
*********************Created By: Surendra********************************
*********************QUERY ID:  entIbsDetail.LoadACHIBSDetail********************************
*********************Comments:  *****************/
IF EXISTS (SELECT 1 
FROM sys.indexes 
WHERE name='IX_IBSH_REPORT_TYPE_STATUS' AND object_id = OBJECT_ID('[dbo].[SGT_IBS_HEADER]'))
DROP INDEX [IX_IBSH_REPORT_TYPE_STATUS] ON [dbo].[SGT_IBS_HEADER]
GO

CREATE NONCLUSTERED INDEX [IX_IBSH_REPORT_TYPE_STATUS]
ON [dbo].[SGT_IBS_HEADER] ([REPORT_TYPE_VALUE],[REPORT_STATUS_VALUE])
INCLUDE ([IBS_HEADER_ID],[BILLING_MONTH_AND_YEAR])
GO
/********************Purpose: Allowing ASCII characters in comments******************************
*********************Created By: Dinesh********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS WHERE SETTING_NAME = 'AllowNonASCIIChars')
INSERT INTO dbo.SGS_SYSTEM_SETTINGS (SETTING_NAME, SETTING_TYPE, SETTING_VALUE, ENCRYPTED_FLAG)
VALUES ('AllowNonASCIIChars','bool','True',NULL)
GO 