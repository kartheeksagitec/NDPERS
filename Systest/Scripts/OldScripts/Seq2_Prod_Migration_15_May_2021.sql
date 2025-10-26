
/********************Purpose: PIR 23837	Add message to MSS when changing bank accounts for Insurance Premium Deductions after pre-note******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments:  *****************/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='UPMW')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'UPMW','Update Payment Method Wizard Acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 23837',GETDATE(),'PIR 23837',GETDATE(),0)
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE ACKNOWLEDGEMENT_TEXT = 'Your election is too late for the next insurance premium to be withheld from this bank account and will be effective {0}. Please remit a personal check for the current premium due.')
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT VALUES(GETDATE(),6000,'UPMW',1,
    'Your election is too late for the next insurance premium to be withheld from this bank account and will be effective {0}. Please remit a personal check for the current premium due.','N','PIR 23837',GETDATE(),'PIR 23837',GETDATE(),0)
GO


/********************Purpose: PIR 18493 - MSS Application wizard ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: change in workflow process *****************/
IF EXISTS(SELECT 1 FROM SGW_PROCESS WHERE PROCESS_ID = 367 AND DESCRIPTION = 'Enroll Retiree Insurance Plans')
	UPDATE SGW_PROCESS SET DESCRIPTION = 'Enroll Retiree Cobra Insurance Plans' WHERE PROCESS_ID = 367





/*Data_Change_PIR_23408 , 23167, 23340 */
/********************Purpose: Added new resources for Enhanced overlap feature******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/

BEGIN
DECLARE @cnt INT
BEGIN TRANSACTION Trans
SELECT @cnt = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2052 AND RESOURCE_DESCRIPTION='Person - Enrollment - Life - Enhanced Overlap'; 
   if @cnt = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2052,12,'U','Person - Enrollment - Life - Enhanced Overlap','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2052 and RESOURCE_DESCRIPTION Person - Enrollment - Life - Enhanced Overlap is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2052

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------


BEGIN
DECLARE @cnt1 INT
BEGIN TRANSACTION Trans
SELECT @cnt1 = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2053 AND RESOURCE_DESCRIPTION='Person - Enrollment - GHDV - Enhanced Overlap'; 
   if @cnt1 = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2053,12,'U','Person - Enrollment - GHDV - Enhanced Overlap','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2053 and RESOURCE_DESCRIPTION Person - Enrollment - GHDV - Enhanced Overlap is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2053

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------


BEGIN
DECLARE @cnt2 INT
BEGIN TRANSACTION Trans
SELECT @cnt2 = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2054 AND RESOURCE_DESCRIPTION='Person - Enrollment - EAP - Enhanced Overlap'; 
   if @cnt2 = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2054,12,'U','Person - Enrollment - EAP - Enhanced Overlap','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2054 and RESOURCE_DESCRIPTION Person - Enrollment - EAP - Enhanced Overlap is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2054

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------


BEGIN
DECLARE @cnt3 INT
BEGIN TRANSACTION Trans
SELECT @cnt3 = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2055 AND RESOURCE_DESCRIPTION='Person - Enrollment - Flex - Enhanced Overlap'; 
   if @cnt3 = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2055,12,'U','Person - Enrollment - Flex - Enhanced Overlap','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2055 and RESOURCE_DESCRIPTION Person - Enrollment - Flex - Enhanced Overlap is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2055

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------


BEGIN
DECLARE @cnt4 INT
BEGIN TRANSACTION Trans
SELECT @cnt4 = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2056 AND RESOURCE_DESCRIPTION='Person - Enrollment - Flex - Allow Overlap'; 
   if @cnt4 = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2056,12,'U','Person - Enrollment - Flex - Allow Overlap','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2056 and RESOURCE_DESCRIPTION Person - Enrollment - Flex - Allow Overlap is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2056

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------


BEGIN
DECLARE @cnt5 INT
BEGIN TRANSACTION Trans
SELECT @cnt5 = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2057 AND RESOURCE_DESCRIPTION='Miscellaneous Correction Third Save Button'; 
   if @cnt5 = 0   
   Begin    
	   INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
	   ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2057,12,'U','Miscellaneous Correction Third Save Button','PIR23408',GETDATE(),'PIR23408',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2057 and RESOURCE_DESCRIPTION Miscellaneous Correction Third Save Button is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END



 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
 BEGIN TRANSACTION Trans2

INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR23408', GETDATE(), 'PIR23408', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2057

COMMIT TRANSACTION Trans2

------------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID=10410)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10410,'Please change Payee Account deduction if necessary.',16,'W',NULL,NULL,'PIR 23408',GETDATE(),'PIR 23408',GETDATE(),0)
GO

--------------------------------------------------------------------------------------------------------------------------------------------------------------
/*End of Data_Change_PIR_23408 , 23167, 23340 */

 --------------------------------------------------------------------------------------------------------------
 /* PIR 11283 Add Warning Message when deposit date and payment dates on Deposits are more than 5 days*/
 ---------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10411)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10411,'Date entered exceeds 5 business days.',16,'E',NULL,NULL,'PIR 11283',GETDATE(),'PIR 11283',GETDATE(),0)
GO