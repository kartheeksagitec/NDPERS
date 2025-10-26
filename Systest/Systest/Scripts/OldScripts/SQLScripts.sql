
--------------Created By: Tushar Chandak 
--------------Purpose   :Create payroll report/Add ESSHelpFiles and ESSForms Paths/Add Columns in SGT_EMPLOYER_PAYROLL_DETAIL table
--------------Date      : 07 April 2014

/* Add code_id 7000 into sgs_code table */
SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] ON
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_CODE] WHERE CODE_ID = 7000)
INSERT dbo.SGS_CODE ([CODE_ID],[DESCRIPTION],[DATA1_CAPTION],[DATA1_TYPE],[DATA2_CAPTION],[DATA2_TYPE],[DATA3_CAPTION],[DATA3_TYPE],[FIRST_LOOKUP_ITEM],[FIRST_MAINTENANCE_ITEM],[COMMENTS],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(7000,'Payroll report options','','','','','','','All',NULL,NULL,NULL,'tchandak',GETDATE(),'tchandak',GETDATE(),0)
GO
SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] OFF
GO

/* Add code_values for code_id 9108 into sgs_code_value table */
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7000 AND CODE_VALUE='CRPR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7000,'CRPR','What type of Payroll Report would you like to create?',null,null,null,null,null,null,null
           ,NULL,'tchandak',GETDATE(),'tchandak',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7000 AND CODE_VALUE='VUPR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7000,'VUPR','View/Update Payroll Report ',null,null,null,null,null,null,null
           ,NULL,'tchandak',GETDATE(),'tchandak',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7000 AND CODE_VALUE='INPR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7000,'INPR','Initiate payment for Payroll Report ',null,null,null,null,null,null,null
           ,NULL,'tchandak',GETDATE(),'tchandak',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7000 AND CODE_VALUE='SCPR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7000,'SCPR','Search Payroll Details',null,null,null,null,null,null,null
           ,NULL,'tchandak',GETDATE(),'tchandak',GETDATE(),0)
GO


/* Add Columns in SGT_EMPLOYER_PAYROLL_DETAIL table  */
    ALTER TABLE SGT_EMPLOYER_PAYROLL_DETAIL
    ADD 
    [EE_CONTRIBUTION_ORIGINAL] [decimal](11, 2) NULL,
	[EE_PRE_TAX_ORIGINAL] [decimal](11, 2) NULL,
	[EE_EMPLOYER_PICKUP_ORIGINAL] [decimal](11, 2) NULL,
	[ER_CONTRIBUTION_ORIGINAL] [decimal](11, 2) NULL,
	[RHIC_ER_CONTRIBUTION_ORIGINAL] [decimal](11, 2) NULL,
	[RHIC_EE_CONTRIBUTION_ORIGINAL] [decimal](11, 2) NULL

/* Add ESSHelpFiles and ESSForms values in SGS_SYSTEM_PATHS table  */
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_SYSTEM_PATHS] WHERE PATH_CODE = 'ESSFORM')
INSERT INTO [SGS_SYSTEM_PATHS] ([PATH_CODE],[PATH_VALUE],[PATH_DESCRIPTION], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], 
[MODIFIED_DATE], [UPDATE_SEQ]) VALUES ('ESSFORM', 'ESSForms\', 'Directory to store the ESS Forms', 'anil', GETDATE(), 'anil', GETDATE() , '0')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_SYSTEM_PATHS] WHERE PATH_CODE = 'ESSHELP')
INSERT INTO [dbo].[SGS_SYSTEM_PATHS] ([PATH_CODE],[PATH_VALUE],[PATH_DESCRIPTION], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) VALUES ('ESSHELP', 'ESSHelpFiles\', 'Directory to store the ESS Help Files', 'anil', GetDate() , 
'anil', GetDate(), '0')

GO

--------------Created By: Tushar Chandak
--------------Purpose   : Create payroll update report type data2
--------------Date      : 09 April 2014
Update SGS_CODE_VALUE set DATA2='CRES' where CODE_ID=1212 and CODE_VALUE<>'INSR'


/* Employee Options */
SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] ON
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_CODE] WHERE CODE_ID = 7001)
INSERT dbo.SGS_CODE ([CODE_ID],[DESCRIPTION],[DATA1_CAPTION],[DATA1_TYPE],[DATA2_CAPTION],[DATA2_TYPE],[DATA3_CAPTION],[DATA3_TYPE],[FIRST_LOOKUP_ITEM],[FIRST_MAINTENANCE_ITEM],[COMMENTS],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(7001,'ESS Employee Options','','','','','','','All',NULL,NULL,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] OFF
GO

/* Add code_values for code_id 9108 into sgs_code_value table */
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='SENE')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'SENE','Set up new Employee',null,null,null,null,null,null,1,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='TEEM')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'TEEM','Terminate Employee',null,null,null,null,null,null,2,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='UPED')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'UPED','Update Employment Details',null,null,null,null,null,null,3,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='VIED')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'VIED','View/Update Employees',null,null,null,null,null,null,4,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='OTRP')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'OTRP','Enroll in Other 457/403(b) Plan ',null,null,null,null,null,null,5,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7001 AND CODE_VALUE='VIEP')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7001,'VIEP','View pending Requests ',null,null,null,null,null,null,6,NULL,'Karthik',GETDATE(),'Karthik',GETDATE(),0)
GO

ALTER TABLE SGT_WSS_EMPLOYMENT_CHANGE_REQUEST
ADD LTC_CONTINUED VARCHAR(4)

ALTER TABLE SGT_WSS_EMPLOYMENT_CHANGE_REQUEST
ADD FLEX_COMP_CONTINUED VARCHAR(4)


--------------Created By: Anil Jadhav
--------------Purpose   : Added Employee Maintenance Options
--------------Date      : 06 JUNE 2014
/* Employee Maintenance Options - Add code_id 7002 into sgs_code table */

SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] ON
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_CODE] WHERE CODE_ID = 7002)
INSERT dbo.SGS_CODE ([CODE_ID],[DESCRIPTION],[DATA1_CAPTION],[DATA1_TYPE],[DATA2_CAPTION],[DATA2_TYPE],[DATA3_CAPTION],[DATA3_TYPE],[FIRST_LOOKUP_ITEM],[FIRST_MAINTENANCE_ITEM],[COMMENTS],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES
(7002,'ESS Employee Maintenance Options','','','','','','','All',NULL,NULL,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO
SET IDENTITY_INSERT [dbo].[SGS_CODE_VALUE] OFF
GO

/* Add code_values for code_id 7002 into sgs_code_value table*/
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7002 AND CODE_VALUE='CLSC')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7002,'CLSC','Update Employment',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7002 AND CODE_VALUE='LOAR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7002,'LOAR','Leave Of Absence – Recertification',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO		   
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7002 AND CODE_VALUE='LOAM')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7002,'LOAM','Leave Of Absence – Military',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO			   
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7002 AND CODE_VALUE='LOAL')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7002,'LOAL','LOA/Leave without pay',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO		   
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 7002 AND CODE_VALUE='TEEM')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(7002,'TEEM','Terminate Employment',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO

--------------Created By: Surendra Reddy
--------------Purpose   : Updates cor_templates for ESS redesign for employee maintenance screen
--------------Date      : 06 June 2014

UPDATE SGS_COR_TEMPLATES SET ASSOCIATED_FORMS = 'WFMESSEMPLOYEEMAINTENANCE;WFMWSSEMPLOYEEMAINTENANCE;' WHERE TEMPLATE_ID = 541

--------------Created By: Anil Jadhav
--------------Purpose   : Added 'Terminate Employment' Employee Maintenance Option.Updated Message for MESSAGE_ID='8539'
--------------Date      : 11 JUNE 2014  

IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 3506 AND CODE_VALUE='TEEM')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(3506,'TEEM','Terminate Employment',null,null,null,null,null,null,null
           ,NULL,'ajadhav',GETDATE(),'ajadhav',GETDATE(),0)
GO

Update SGS_MESSAGES
set DISPLAY_MESSAGE='Employment for {0} has been successfully posted.Please have employee enroll in their benefit plans through MSS.'
where MESSAGE_ID='8539'

Update sgs_file
set description='Employer Report - Purchases Outbound File',XML_LAYOUT_FILE='fleExportPurchaseOut'
where File_id=19
 
 
--------------Created By: Mohasin momin
--------------Purpose   : PIR 13188- Added Zip 4 Code to Death Notice Table
--------------Date      : 14 JULY 2014

ALTER TABLE dbo.SGT_DEATH_NOTICE ADD
	CONTACT_ZIP_4_CODE varchar(4) NULL
GO


--------------Created By: Mohasin momin
--------------Purpose   : PIR 13172- Added new message.
--------------Date      : 16 JULY 2014
 
 INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10224
           ,'Employment Change request for {0} was processed with errors. This request appears below.  Please review the reason for the error. Then click on the Request ID link to make any necessary changes and resubmit.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,'2014-07-08 17:49:11.880'
           ,'!mshinde'
           ,'2014-07-08 17:49:11.880'
           ,0)
GO

UPDATE [dbo].[SGS_MESSAGES]
   SET 
      [DISPLAY_MESSAGE] = 'New Employee request for {0} was processed with errors. This request appears below.  Please review the reason for the error. Then click on the Request ID link to make any necessary changes and resubmit.'
 WHERE [MESSAGE_ID] = 8541
GO
 
--------------Created By: Surendra Reddy
--------------Purpose   : PIR 12753,13214 -Added new message.
--------------Date      : 17 JULY 2014

INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10225, 'Last Reporting Month for Retirement Contributions is required.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'Last Date of Service should be same or greater than Hire Date.' WHERE MESSAGE_ID = 8584

--------------Created By: Tushar Chandak
--------------Purpose   : PIR 13208 -Updated Code value description.
--------------Date      : 21 JULY 2014

UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'Create Payroll Report' 
WHERE CODE_ID=7000 and CODE_VALUE='CRPR'

--------------Created By: Surendra Reddy
--------------Purpose   : PIR 13278 - To filter out an option from the dropdown
--------------Date      : 28 JULY 2014

UPDATE SGS_CODE_VALUE SET DATA2 = 'Y' WHERE CODE_ID = 1201 AND CODE_VALUE != 'FLEX'

--------------Created By: Mohasin momin
--------------Purpose   : PIR-13211 - Added new messages.
--------------Date      : 8 August 2014

INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10226, 'Please select Is employee Hourly?.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)


INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10227, 'Please enter either home phone number or cell phone number.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10228
           ,'Spouse First Name is required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10229
           ,'Spouse Last Name is required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10230
           ,'Employment Status is required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10231
           ,'Participation Status is required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10232
           ,'Employee’s Date of Hire is required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10233
           ,'Employee’s Date of Hire cannot be less than or equal to Date of Birth'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10234
           ,'Selection is required for ''Does employee work less than 12 months per year?'''
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10235
           ,'Selection is required for ''Is employee Hourly?'''
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10236
           ,'Appointed Official is required if job class is Appointed Official.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

UPDATE SGS_CODE_VALUE SET DESCRIPTION='Appointed by Law' WHERE CODE_SERIAL_ID=12574

Go

--------------Created By: Surendra Reddy
--------------Purpose   : PIR-13214-This field needed to be shown on employment change request lookup
--------------Date      : 11 August 2014

Alter table SGT_WSS_EMPLOYMENT_CHANGE_REQUEST
Add IS_ON_TEACHING_CONTRACT_ID INT NOT NULL DEFAULT(311)

Alter table SGT_WSS_EMPLOYMENT_CHANGE_REQUEST 
Add IS_ON_TEACHING_CONTRACT_VALUE VARCHAR(4)

--------------Created By: Surendra Reddy
--------------Purpose   : PIR-13278-Wire transfer option not to be shown on lookup.
--------------Date      : 27 August 2014

UPDATE SGS_CODE_VALUE SET DATA1 = 'Y' WHERE CODE_ID = 1501 AND CODE_VALUE <> 'WITR'

--------------Created By: Mohasin momin
--------------Purpose   : PIR-13171- Added ADDRESS_VALIDATE_ERROR and ADDRESS_VALIDATE_FLAG columns to ORGANIZATION contact address table.
--------------            This is used for address VALIDATION.
--------------Date      : 10 Sept 2014

ALTER TABLE dbo.SGT_ORG_CONTACT_ADDRESS ADD
	ADDRESS_VALIDATE_FLAG char(4) NULL,
	ADDRESS_VALIDATE_ERROR varchar(100) NULL
GO

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE='Address is Invalid. Please enter a valid Address.' WHERE MESSAGE_ID=127

Go

--------------Created By: Anil Jadhav
--------------Purpose   : PIR-13157- Added new message.Updated messages for Description & Data1 to filter out an option from the dropdown.
--------------Date      : 16 Sept 2014

UPDATE SGS_CODE_VALUE SET DATA1='Y' WHERE CODE_ID=7002 AND CODE_VALUE IN ('LOAL','CLSC'); 

UPDATE SGS_CODE_VALUE SET DESCRIPTION='Leave Of Absence' WHERE CODE_ID=7002 AND CODE_VALUE='LOAL' 

INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS],
[EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10237, 'Please select a type of leave.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)

INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS],
[EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10238, 'Enter either Return from LOA Date or Recertification Date.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde',
GETDATE(), 0)
 
INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS],
[EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10239, 'A Leave of Absence cannot be exceed one year without being recertified.Enter recertification date.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)

--------------Created By: Surendra Reddy/ Mohasin Momin.
--------------Purpose   : PIR-13190- Added new message. ID referred in XML but no message in Messages table
--------------			Added ADDRESS_VALIDATE_FLAG and ADDRESS_VALIDATE_ERROR columns to death notice for address validation.
--------------Date      : 16 Sept 2014

		INSERT INTO [dbo].[SGS_MESSAGES]
				   ([MESSAGE_ID]
				   ,[DISPLAY_MESSAGE]
				   ,[SEVERITY_ID]
				   ,[SEVERITY_VALUE]
				   ,[INTERNAL_INSTRUCTIONS]
				   ,[EMPLOYER_INSTRUCTIONS]
				   ,[CREATED_BY]
				   ,[CREATED_DATE]
				   ,[MODIFIED_BY]
				   ,[MODIFIED_DATE]
				   ,[UPDATE_SEQ])
			 VALUES
				   (10088
				   ,'Requested Date From cannot be greater than Requested Date To.'
				   ,16
				   ,'E'
				   ,NULL
				   ,NULL
				   ,'!mshinde'
				   ,getdate()
				   ,'!mshinde'
				   ,getdate()
				   ,0)
		   
 GO
		INSERT INTO [dbo].[SGS_MESSAGES]
				   ([MESSAGE_ID]
				   ,[DISPLAY_MESSAGE]
				   ,[SEVERITY_ID]
				   ,[SEVERITY_VALUE]
				   ,[INTERNAL_INSTRUCTIONS]
				   ,[EMPLOYER_INSTRUCTIONS]
				   ,[CREATED_BY]
				   ,[CREATED_DATE]
				   ,[MODIFIED_BY]
				   ,[MODIFIED_DATE]
				   ,[UPDATE_SEQ])
			 VALUES
				   (10243
				   ,'Please Enter Last Month of Coverage.'
				   ,16
				   ,'E'
				   ,NULL
				   ,NULL
				   ,'!mshinde'
				   ,getdate()
				   ,'!mshinde'
				   ,getdate()
				   ,0)
GO		   
		INSERT INTO [dbo].[SGS_MESSAGES]
				   ([MESSAGE_ID]
				   ,[DISPLAY_MESSAGE]
				   ,[SEVERITY_ID]
				   ,[SEVERITY_VALUE]
				   ,[INTERNAL_INSTRUCTIONS]
				   ,[EMPLOYER_INSTRUCTIONS]
				   ,[CREATED_BY]
				   ,[CREATED_DATE]
				   ,[MODIFIED_BY]
				   ,[MODIFIED_DATE]
				   ,[UPDATE_SEQ])
			 VALUES
				   (10246
				   ,'Description of Problem is required.'
				   ,16
				   ,'E'
				   ,NULL
				   ,NULL
				   ,'!mshinde'
				   ,getdate()
				   ,'!mshinde'
				   ,getdate()
				   ,0)


GO

ALTER TABLE dbo.SGT_DEATH_NOTICE ADD
	ADDRESS_VALIDATE_FLAG char(4) NULL,
	ADDRESS_VALIDATE_ERROR varchar(100) NULL
GO

--------------Created By: Mohasin momin
--------------Purpose   : PIR-13381- Added WEBCAST_AVAILABLE column to seminar schedule table. 
--------------                       Added NO_OF_WEBCAST_ATTENDEES column to SGT_SEMINAR_ATTENDEE_DETAIL table.
--------------Date      : 19 Sept 2014

ALTER TABLE dbo.SGT_SEMINAR_SCHEDULE ADD
	WEBCAST_AVAILABLE char(1) NULL
GO
ALTER TABLE dbo.SGT_SEMINAR_ATTENDEE_DETAIL ADD
	NO_OF_WEBCAST_ATTENDEES int NULL
GO

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10240)
 INSERT [SGS_MESSAGES] ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (10240, 'This report cannot be created without details entered.', 16, 'E', NULL, NULL, '!mshinde', GETDATE(), '!mshinde', GETDATE(), 0)
GO

--------------Created By: Mohasin momin
--------------Purpose   : PIR 13648 Created New Corrospondence Template
--------------Date      : 4 Nov 2014
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_COR_TEMPLATES] WHERE TEMPLATE_ID = '544')
INSERT INTO [dbo].[SGS_COR_TEMPLATES] ([TEMPLATE_NAME], [TEMPLATE_DESC], [TEMPLATE_GROUP_ID], [TEMPLATE_GROUP_VALUE], [ACTIVE_FLAG], [DESTINATION_ID], [DESTINATION_VALUE], 
[ASSOCIATED_FORMS], [FILTER_OBJECT_ID], [FILTER_OBJECT_FIELD], [FILTER_OBJECT_VALUE], [CONTACT_ROLE_ID], [CONTACT_ROLE_VALUE], [BATCH_FLAG], [ONLINE_FLAG], [AUTO_PRINT_FLAG], [PRINTER_NAME_ID], 
[PRINTER_NAME_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ],
IMAGE_DOC_CATEGORY_ID,IMAGE_DOC_CATEGORY_VALUE,FILENET_DOCUMENT_TYPE_ID,FILENET_DOCUMENT_TYPE_VALUE,DOCUMENT_CODE) 
VALUES ( 'SFN-60711', 'SFN 60711 Decline Offer of Health Insurance Coverage', '19', 'MMBR', 'Y', '601', NULL, 
'wfmHealthDentalVisonMaintenance;wfmPersonDependentMaintenance;wfmPersonMaintenance;', NULL, NULL, NULL, 
'515',NULL, 'N', 'Y', 'N', '44', NULL,  '!mshinde', getdate(), '!mshinde', getdate(), '0',
603,'MEMB',604,'FORM',60711)
GO

--------------Created By: Anil Jadhav
--------------Purpose   : Delete LOA Report option from the dropdown.
--------------Date      : 29 Oct 2014
DELETE from SGS_CODE_VALUE where CODE_ID=7001 and CODE_VALUE='REPT'


INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10244
           ,'End Month for Bonus should be greater than Reporting Month.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


--------------Created By: Anil Jadhav
--------------Purpose   : Used to sort data in Missing Retirement Contribution Report.
--------------Date      : 13 Nov 2014
UPDATE SGS_CODE_VALUE SET DATA3=1 where CODE_ID=408 and CODE_VALUE ='BASC'
UPDATE SGS_CODE_VALUE SET DATA3=2 where CODE_ID=408 and CODE_VALUE ='SPML'
UPDATE SGS_CODE_VALUE SET DATA3=3 where CODE_ID=408 and CODE_VALUE ='DSPL'
UPDATE SGS_CODE_VALUE SET DATA3=4 where CODE_ID=408 and CODE_VALUE ='SPSL'



--------------Created By: Surendra Reddy
--------------Purpose   : Payroll Changes - Added Original Amount Fields for deferred comp and service purchase headers.
--------------Date      : 20 October 2014

ALTER TABLE SGT_EMPLOYER_PAYROLL_DETAIL
    ADD
	[CONTRIBUTION_AMOUNT1_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID1_ORIGINAL] VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT2_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID2_ORIGINAL]  VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT3_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID3_ORIGINAL]  VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT4_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID4_ORIGINAL]  VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT5_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID5_ORIGINAL]  VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT6_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID6_ORIGINAL]  VARCHAR(50) NULL,
	[CONTRIBUTION_AMOUNT7_ORIGINAL] [decimal](11, 2) NULL,
	[PROVIDER_ORG_CODE_ID7_ORIGINAL]  VARCHAR(50) NULL,
	[PAYMENT_CLASS_ORIGINAL_ID] INT,
	[PAYMENT_CLASS_ORIGINAL_VALUE] VARCHAR(4) NULL,
	[PURCHASE_AMOUNT_ORIGINAL] [decimal](11, 2) NULL
GO

 ALTER TABLE SGT_EMPLOYER_PAYROLL_DETAIL
    ADD 
    [WAGES_ORIGINAL] [decimal](11, 2) NULL

--------------Created By: Surendra Reddy
--------------Purpose   : Payroll Changes - Maik Mail dated November 28, 2012
--------------Date      : 01 December 2014
IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 52 AND CODE_VALUE='RDVE')
INSERT [SGS_CODE_VALUE] 
VALUES (52, N'RDVE', N'Rounding Difference Value', '1.00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'!mshinde', GETDATE(), N'!mshinde', GETDATE(), 0)
GO


--------------Created By: Saurabh Churadiya
--------------Purpose   : Table Update, New Coloumn added.
--------------Date      : 24 November 2014

ALTER TABLE sgt_organization
ADD TELEPHONE_NO_EXTN VARCHAR (50)

--------------Created By: Mohasin momin
--------------Purpose   : New Messages Added
--------------Date      : 4 Nov 2014

delete from SGS_CODE_VALUE where CODE_ID=3510 AND CODE_VALUE='NA'

GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10241
           ,'Please select NDPERS insurance plans.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10242
           ,'Recertification Date should be at least one year after LOA Start Date.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


--------------Created By: Saurabh Churadiya
--------------Purpose   : PIR 13380 Phone Number !< 10
--------------Date      : 17 Dec 2014
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10247)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10247
           ,'Phone Numbers cannot be less than 10 digits.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


--------------Created By: Saurabh Churadiya
--------------Purpose   : PIR 13882 Message update For Person Id
--------------Date      : 25 Dec 2014
UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'Employment changes for {0}, {1} are successfully posted' WHERE MESSAGE_ID = 8540
GO

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'Employment for {0}, {1} has been successfully posted.Please have employee enroll in their benefit plans through MSS.' WHERE MESSAGE_ID = 8539
GO


--------------Created By: Anil Jadhav
--------------Purpose   : Delete & update Code Value Description.
--------------Date      : 17 Dec 2014
DELETE FROM SGS_CODE_VALUE WHERE CODE_ID = 7001 AND CODE_VALUE = 'UPED'
UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'View / Update Employees' WHERE CODE_ID = 7001 AND CODE_VALUE = 'VIED'
UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'Under NDPERS Review' WHERE CODE_ID = 3508 AND CODE_VALUE = 'REVW'
UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'View Payroll Reports' WHERE CODE_ID = 7000 AND CODE_VALUE = 'VUPR'
UPDATE SGS_CODE_VALUE SET DESCRIPTION = 'Bonus / Retro Pay' WHERE CODE_ID = 1208 AND CODE_VALUE = 'BONS'

--------------Created By: Anil Jadhav
--------------Purpose   : Delete & update Code Value Description.
--------------Date      : 17 Dec 2014
 ALTER TABLE SGT_EMPLOYER_PAYROLL_HEADER
    ADD 
    [TOTAL_WAGES_ORIGINAL] [decimal](11, 2) NULL,
	[TOTAL_CONTRIBUTION_ORIGINAL] [decimal](11, 2) NULL,
	[TOTAL_PURCHASE_AMOUNT_ORIGINAL] [decimal](11, 2) NULL

GO


--------------Created By: Snehal Wagh
--------------Purpose   : PIR 13996 Eligible Wages
--------------Date      : 20 Jan 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10248)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10248
			,'For Record Type Negative Adjustment Eligible Wages Should be Negative.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO

--------------Created By: Mohasin Momin
--------------Purpose   : PIR 13996 Reporting Month validation
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10249)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10249
           ,'Reporting month should be greater than or equal to 01/1977 on line(s) {0}'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

--------------Created By: Mohasin Momin
--------------Purpose   : PIR 13996 Begin Month validation for Bonus/Retro pay
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10250)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10250
           ,'Begin month for Bonus/Retro pay should be greater than or equal to 01/1977 on line(s) {0}'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

--------------Created By: Mohasin Momin
--------------Purpose   : PIR 13996 End Month validation for Bonus/Retro pay
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10251)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10251
           ,'End month for Bonus/Retro pay should be greater than or equal to 01/1977 on line(s) {0}'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


--------------Created By: Mohasin Momin
--------------Purpose   : PIR 13996 Reporting Month validation
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10252)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10252
           ,'Reporting month for Retirement Adjustment should not be less than Jan 1977.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO


--------------Created By: Mohasin Momin
--------------Purpose   : PIR 13996 Begin/End Month validation for Bonus/Retro pay
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10253)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10253
           ,'Begin month or End month for Retirement Bonus/Retro pay should not be less than Jan 1977.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'!mshinde'
           ,getdate()
           ,'!mshinde'
           ,getdate()
           ,0)
GO

--------------Created By: Tushar Chandak
--------------Purpose   : PIR 13940 Eligible Wages
--------------Date      : 06 Feb 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10254)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10254
			,'Total Wages Reported and Total contribution reported are required.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10255)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10255
			,'Total contribution reported are required.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10256)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10256
			,'Total Purchase Reported are required.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10257)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10257
			,'Eligible Wages must be Positive.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10258)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10258
			,'Eligible Wages must be Negative.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10259)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10259
			,'Please enter positive Eligible Wages on line {0}.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10260)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10260
			,'Total Wages Reported and Total contribution reported must be positive.'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
--------------Created By: Surendra Reddy
--------------Purpose   : PIR 14042 Report and Detail Benefit Type Order
--------------Date      : 10 March 2015

UPDATE SGS_CODE_VALUE SET DATA3 = '1' WHERE CODE_VALUE = 'RETR' AND CODE_ID = 1212
UPDATE SGS_CODE_VALUE SET DATA3 = '2' WHERE CODE_VALUE = 'INSR' AND CODE_ID = 1212
UPDATE SGS_CODE_VALUE SET DATA3 = '3' WHERE CODE_VALUE = 'DEFF' AND CODE_ID = 1212
UPDATE SGS_CODE_VALUE SET DATA3 = '4' WHERE CODE_VALUE = 'PRCH' AND CODE_ID = 1212

--------------Created By: Rohit Shah
--------------Purpose   : PIR 14043 Leave of Absence
--------------Date      : 11 March 2015
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10262)
INSERT INTO [dbo].[SGS_MESSAGES] 
            ([MESSAGE_ID]
			,[DISPLAY_MESSAGE]
			,[SEVERITY_ID]
			,[SEVERITY_VALUE]
			,[INTERNAL_INSTRUCTIONS]
			,[EMPLOYER_INSTRUCTIONS]
			,[CREATED_BY]
			,[CREATED_DATE]
			,[MODIFIED_BY]
			,[MODIFIED_DATE]
			,[UPDATE_SEQ])
       VALUES
	        (10262
			,'LOA reason for leave is required'
			,16
			,'E'
			,NULL
			,NULL
			,'!mshinde'
			,getdate()
			,'!mshinde'
			,getdate()
			,0)
GO
--------------Created By: Surendra Reddy
--------------Purpose   : PIR 14042 Displaying Benefit based record types
--------------Date      : 10 March 2015
UPDATE SGS_CODE_VALUE SET DATA2 = 'Y' WHERE CODE_ID = 1208 AND CODE_VALUE != 'PURC'
GO
UPDATE SGS_CODE_VALUE SET DATA3 = 'Y' WHERE CODE_ID = 1208 AND CODE_VALUE NOT IN ('PURC', 'BONS')
GO

--------------Created By: Rohit Shah
--------------Purpose   : PIR 14216
--------------Date      : 20 March 2015
CREATE TABLE [SGS_FILE_HDR_ERROR](
	[ERROR_ID] [int]  IDENTITY(1,1) NOT NULL,
	[FILE_HDR_ID] [int] NULL,
	[ERROR_MESSAGE_ID] [varchar](256) NULL,
	[ERROR_MESSAGE] [varchar](500) NULL,
	[CREATED_BY] [varchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[MODIFIED_BY] [varchar](50) NOT NULL,
	[MODIFIED_DATE] [datetime] NOT NULL,
	[UPDATE_SEQ] [int] NOT NULL,
 CONSTRAINT [PK_SGS_FILE_HDR_ERROR_ERROR_ID] PRIMARY KEY CLUSTERED 
  (
	[ERROR_ID] ASC
  )
)
GO

--------------Created By: Snehal Wagh
--------------Purpose   : PIR 14227 
--------------Date      : 06 April 2015

IF NOT EXISTS (SELECT * FROM [SGS_CODE_VALUE] WHERE CODE_ID = 52 AND CODE_VALUE='HENL')
INSERT [dbo].[SGS_CODE_VALUE] ([CODE_SERIAL_ID], [CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], [DATA3],
             [COMMENTS], [START_DATE], [END_DATE], [CODE_VALUE_ORDER], [LEGACY_CODE_ID], [CREATED_BY], [CREATED_DATE],
			 [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
      VALUES (17819, 52, N'HENL', N'Apply to Health Enrollment', N'N', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'!MShinde', 
	          GETDATE(), N'!MShinde', GETDATE(), 0)

GO