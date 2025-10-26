--------------Created By: Vidya Fulsoundar
--------------Purpose   : PIR 25221
--------------Date      : 07/24/2023
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID=10494)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10494, 'You cannot enroll with the Pre Tax option during this plan year.', 16, 'E',	NULL,	NULL,	'PIR 25221',	GETDATE(),	'PIR 25221',	GETDATE(),	0)
END

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10462)
BEGIN
	INSERT INTO SGS_MESSAGES VALUES(10462,	'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.', 16, 'E', NULL,	NULL, 'PIR 25221', GETDATE(), 'PIR 25221', GETDATE(), 0)
END
ELSE IF EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10462 AND DISPLAY_MESSAGE <> 'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.')
BEGIN
	UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.' WHERE MESSAGE_ID = 10462
END
--------------Created By: Vidya Fulsoundar
--------------Purpose   : PIR 18150
--------------Date      : 08/28/2023
IF EXISTS (SELECT 1 FROM [SGT_RETRO_ITEM_TYPE] WHERE RETRO_PAYMENT_TYPE_VALUE = 'RADJ' AND FROM_ITEM_TYPE = 'ITEM38' AND PAYMENT_OPTION_VALUE = 'REGL')
BEGIN
UPDATE  [SGT_RETRO_ITEM_TYPE] SET TO_ITEM_TYPE = 'ITEM49' WHERE RETRO_PAYMENT_TYPE_VALUE = 'RADJ' and FROM_ITEM_TYPE = 'ITEM38' AND PAYMENT_OPTION_VALUE = 'REGL'
END


--------------Created By: Bharat Reddy
--------------Purpose   : PIR 23482 (Add new resource)
--------------Date      : 08/30/2023
----------------------------- START --------------------------------------
BEGIN
DECLARE @cnt INT
BEGIN TRANSACTION Trans
SELECT @cnt = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2085 AND RESOURCE_DESCRIPTION = 'Bypass Email Verification'; 
   if @cnt = 0   
   Begin    
       INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
       ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2086,12,'U','Bypass Email Verification','PIR 23482',GETDATE(),'PIR 23482',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2086 and RESOURCE_DESCRIPTION "Bypass Email Verification" already exists in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END

------Resource attached to all roles-----------
BEGIN TRANSACTION Trans1
IF NOT EXISTS(SELECT 1 FROM SGS_SECURITY WHERE RESOURCE_ID = 2086)
BEGIN
INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR 23482', GETDATE(), 'PIR 23482', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2086
END
COMMIT TRANSACTION Trans1

----------------------------- END --------------------------------------
--------------Purpose   : PIR 23482 (Update email format)
IF EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS where  SYSTEM_SETTING_Id = 48)
BEGIN
UPDATE SGS_SYSTEM_SETTINGS SET SETTING_VALUE = '<table><tr><td>{0},</td></tr><tr><td><br>Please enter this code to save the changes to your NDPERS Member Self Service (MSS) online account: {1}</td></tr><tr><td><b>This code expires in 30  minutes.</b></td></tr><tr><td>If you did not ask for, or were not prompted for an activation code, please contact NDPERS for immediate assistance by calling us at 701-328-3900.</td></tr><tr><td>If you are unable to enter your activation code within 30 minutes, please log in to your MSS account and click on the Resend Activation Code button to receive a new activation code.</td></tr></table>'
WHERE SYSTEM_SETTING_ID = 48
END
GO

IF EXISTS (SELECT 1 FROM SGS_SYSTEM_SETTINGS where  SYSTEM_SETTING_Id = 52)
BEGIN
UPDATE SGS_SYSTEM_SETTINGS SET SETTING_VALUE = '<table><tr><td>Thank you for allowing us to assist you.</td></tr><tr><td><br>Sincerely,<br><b>North Dakota Public Employees Retirement System</b></td></tr><tr><td><p>   1600 East Century Avenue | PO Box 1657<br>  Bismarck, ND 58502| Online <a href=/"https://www.ndpers.nd.gov////">https://www.ndpers.nd.gov/</a></p></td></tr></table><p>Do not reply to this message. </p><p>This e-mail, including any attachments, may contain information that is proprietary,   privileged and/or confidential and is intended exclusively for the person(s) to whom it is addressed. Any use, disclosure, copying, retention or distribution by any person other than the intended recipient or the intended recipient’s designees is strictly prohibited.  If you are not the intended recipient or their designee, please notify the sender immediately by return e-mail and delete all copies.  This message is not intended to provide specific advice or recommendations for any individuals. NDPERS is governed by the laws and regulations set forth in the N.D.C.C. and N.D.A.C. Consult your attorney, accountant, financial or tax advisor about your individual situation. NDPERS has taken reasonable precautions to ensure no viruses are present in this email; however, the agency cannot accept responsibility for any loss or damage that may arise from the use of this email or attachments.</p>'
WHERE SYSTEM_SETTING_ID = 52
END
GO

--------------Created By: Sanket Chougale
--------------Purpose   : PIR 26021
--------------Date      : 08/30/2023

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10495)
INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(10495,'Spouse Supplemental cannot be Enrolled as Dependent Supplemental amount is zero.',16,'E',NULL,NULL,'PIR 26021',GETDATE(),'PIR 26021',GETDATE(),0)
GO