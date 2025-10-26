--------------Created By: sanket chougale
--------------Purpose   : PIR 25655 
--------------Date      : 07 April 2023

Alter table SGT_BENEFIT_APPLICATION_BENEFICIARY alter column DIST_PERCENT decimal(11,3)
Alter table SGT_BENEFIT_DRO_BENEFICIARY alter column DIST_PERCENT decimal(11,3)

--------------Created By: Vidya Fulsoundar	
--------------Purpose   : PIR 24520 
--------------Date      : 26 June 2023
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10490)
BEGIN
INSERT INTO SGS_MESSAGES VALUES(10490,	'Check account status as Account is Processed, Suspended, or Payment Complete.',	16,	'E',	NULL,	NULL,	'PIR 24520',	getdate(),	'PIR 24520',	GETDATE(),	0)
end
Go

--------------Created By: Vidya Fulsoundar	
--------------Purpose   : PIR 25221 
--------------Date      : 26 June 2023
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10462)
BEGIN
	INSERT INTO SGS_MESSAGES VALUES(10462,	'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.', 16, 'E', NULL,	NULL, 'PIR 25221', GETDATE(), 'PIR 25221', GETDATE(), 0)
END
ELSE IF EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10462 AND DISPLAY_MESSAGE <> 'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.')
BEGIN
	UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE = 'You cannot enroll in flexcomp medical, dependent care, or premium conversion during this plan year.' WHERE MESSAGE_ID = 10462
END
Go

/********************Purpose: PIR 18493******************************
*********************Created By: Abhijeet********************************
*********************Comments: New code value for App wizard *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'RFND')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3],[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES
           (7014 ,'RFND','Apply to Refund/Rollover my entire Account Balance',null,null,null,null,null,null,1,null,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO 
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'RETR')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3],[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES
           (7014 ,'RETR','Apply for Retirement (or Defer retirement to a later date)',null,null,null,null,null,null,2,null,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO 
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'DISA')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3],[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES
           (7014 ,'DISA','Apply for Disability',null,null,null,null,null,null,3,null,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO 

IF EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'SARC' AND CODE_VALUE_ORDER = 3)
	UPDATE SGS_CODE_VALUE SET CODE_VALUE_ORDER = 5 WHERE CODE_ID = 7014 AND CODE_VALUE = 'SARC'
GO 

IF EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'VSRA' AND CODE_VALUE_ORDER = 2)
	UPDATE SGS_CODE_VALUE SET CODE_VALUE_ORDER = 4 WHERE CODE_ID = 7014 AND CODE_VALUE = 'VSRA'
GO 

IF EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'ARPB')
	DELETE SGS_CODE_VALUE WHERE CODE_ID = 7014 AND CODE_VALUE = 'ARPB'
GO 


IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='RFAK')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'RFAK','Refund ACH acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'RFAK' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'RFAK',1,'I acknowledge that by receiving a refund/rollover I forfeit all service credit to the date of the distribution, as well as any retirement or disability benefits, and any non-vested employer contributions attributable to that service credit. I have read and understand the <a href="https://www.ndpers.nd.gov/sites/www/files/documents/forms/active-defined-benefit/refund-rollover-forms.pdf" target="_blank"><u>"Safe Harbor Tax Notice Regarding Plan Payments"</u></a> and confirm the information regarding my bona fide termination and eligibility to receive a distribution is accurate.', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END

IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='AHAK')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'AHAK','ACH Direct Deposite acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'AHAK' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'AHAK',1,'I authorize the North Dakota Public Employees Retirement System (NDPERS), third party administrators (TPAs), and the financial institution named on this form to initiate electronic fund transfer (EFT) of my retirement benefit(s) into my account as indicated. I consent to the financial institution sharing my customer information with NDPERS and/or TPAs for the purpose of completing the EFT arrangement.<br/>
I authorize NDPERS and/or TPA to initiate, a reversal or debit entry for all or any portion of any credit entry made in error to my designated account, including but not limited to amounts transferred after my death. If the funds remaining in the designated account are insufficient to fully reimburse NDPERS and/or TPA for any credit entry made in error subsequent to my death, I authorize my financial institution to release to NDPERS and/or TPA any information in its possession regarding the manner and party responsible for any withdrawal or transfer of funds from the designated account made subsequent to the date of the credit entry made in error. <br/>
I authorize my financial institution to notify NDPERS and/or TPA of my death.  This authorization will remain in effect until I notify NDPERS and/or TPA in writing to cancel it in such time as to afford NDPERS and/or TPA a reasonable opportunity to act on it.  I understand any change to my direct deposit authorization must be submitted to the NDPERS office by the 15th of the month prior to the month I want this change to occur.  I agree to the terms listed on this authorization.', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END

IF EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'HP' )
BEGIN
UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 
'I understand that if I am terminating employment and was covered under the NDPERS group health insurance as an active employee, or individuals otherwise losing eligibility, that I may continue my NDPERS Group Health Coverage at my own expense subject to the following:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;1) I must be a member of the plan at time of loss of eligibility.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;2) My spouse or any other dependent(s) applying for this continuation coverage must be a member of the plan at the time of loss of eligibility.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;3) I must complete and submit this election to NDPERS within 60 days from your last date of coverage.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;4) I must not have a lapse in coverage, i.e. premiums must be paid to ensure continuous coverage.<br/><br/>

I understand that continuation of coverage will not be effective until the initial premium payment is received.   If I do not choose continuation coverage, my group health insurance coverage will end on the last day of the month for which premiums were paid.  NDPERS does not direct bill for premiums, so I am responsible for submitting payment by the 1st of each month. <b> Failure to remit your premium by the due date will result in loss of health coverage.</b>'
WHERE SCREEN_STEP_VALUE = 'HP' 
END


IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='HPRT')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'HPRT','Health plan retirement acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'HPRT' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'HPRT',1,
'I understand that if I am eligible, my spouse or my dependents are eligible for Medicare and indicated “Yes” I am eligible, that in order to continue or be eligible for coverage I MUST submit a photocopy of the applicable Medicare ID card(s) for both Parts A & B and complete the NDPERS Medicare Prescription Drug Plan (PDP) Individual Enrollment Form. Therefore, I understand that I or any eligible Medicare member must have Medicare Part B of Medicare. Application for the Medicare Part D Plan cannot be singed or submitted more than 90 days prior to the requested effective date of coverage.<br/>

I authorize the Social Security Administration to furnish the NDPERS Health Plan Provider with medical or other information acquired under the Title XVIII Program (MEDICARE) during the periods my contracts are in force. I authorize the NDPERS Health Plan Provider, or its agent to receive medical information from physicians, hospitals, and other health care providers in order to assure appropriateness of claims payment.'
, 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
GO


IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='HPCN')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'HPCN','Health plan cancellation acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'HPCN' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'HPCN',1,
'To cancel NDPERS group insurance coverage, a written cancellation request must be submitted by the end of the month prior to the effective date. The request must provide my name, last four digits of social security number, NDPERS Member ID and effective date.  Cancellations will only be done at the end of the month. Partial month or retroactive cancellations will not be accepted. <br/>
To the best of my knowledge and belief, the information I have provided is correct. I understand that any person who knowingly and with intent to defraud, submits an application or files a claim containing any materially false or misleading information, commits a fraudulent act, which is a crime. I understand my coverage begins on the effective date assigned by the carrier.  <br/>

I have read and reviewed my eligibility for coverage, the cancellation policy and certify the information is accurate and complete. I understand and agree that any false statements or omissions may constitute a fraudulent act or intentional misrepresentation and may void or retroactively cancel any benefit issued based on this application.'
, 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
GO

IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='HPCR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'HPCR','Health plan retirement cancellation acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'HPCR' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'HPCR',1,
'I understand a NDPERS Disenrollment form is required for any individual on Medicare.  If canceling coverage, I understand I will be responsible to request reimbursement from RHIC vendor for my retiree health insurance credit, if any.'
, 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
GO


IF EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'DP' )
BEGIN
UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 
'I understand that continuation of coverage will not be effective until the initial premium payment is received.   If I do not choose continuation coverage, my group health insurance coverage will end on the last day of the month for which premiums were paid.  NDPERS does not direct bill for premiums, so I am responsible for submitting payment by the 1st of each month. <b>Failure to remit your premium by the due date will result in loss of health coverage.</b>'
WHERE SCREEN_STEP_VALUE = 'DP'  
END

IF EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'VP' )
BEGIN
UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 
'I understand that continuation of coverage will not be effective until the initial premium payment is received.   If I do not choose continuation coverage, my group health insurance coverage will end on the last day of the month for which premiums were paid.  NDPERS does not direct bill for premiums, so I am responsible for submitting payment by the 1st of each month. <b>Failure to remit your premium by the due date will result in loss of health coverage.</b>'
WHERE SCREEN_STEP_VALUE = 'VP' 
END

IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='DPCN')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'DPCN','Dental Vision plan cancellation acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'DPCN' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'DPCN',1,
'To cancel NDPERS group insurance coverage, a written cancellation request must be submitted by the end of the month prior to the effective date. The request must provide my name, last four digits of social security number, NDPERS Member ID and effective date.  Cancellations will only be done at the end of the month. Partial month or retroactive cancellations will not be accepted. <br/>
To the best of my knowledge and belief, the information I have provided is correct. I understand that any person who knowingly and with intent to defraud, submits an application or files a claim containing any materially false or misleading information, commits a fraudulent act, which is a crime. I understand my coverage begins on the effective date assigned by the carrier.  <br/>
I have read and reviewed my eligibility for coverage, the cancellation policy and certify the information is accurate and complete. I understand and agree that any false statements or omissions may constitute a fraudulent act or intentional misrepresentation and may void or retroactively cancel any benefit issued based on this application.'
, 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
GO


IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='DPCR')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'DPCR','Dental Vision plan retirement cancellation acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'DPCR' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'DPCR',1,
'If canceling coverage, I understand I will be responsible to request reimbursement from RHIC vendor for my retiree health insurance credit, if any.'
, 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
GO

