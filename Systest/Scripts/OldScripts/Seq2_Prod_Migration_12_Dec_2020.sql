
/********************Purpose: PIR 18493 App Wizard Round 3 Testing ******************************
*********************Created By: Abhijeet M ********************************
*********************Comments: Added new acknoledgment *****************/
/*Health plan acknoledgement*/
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='HP' AND ACKNOWLEDGEMENT_TEXT ='I, <b>{0}</b>, acknowledge I have had an opportunity to review the terms and conditions relating to participation in Health' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'I understand that if I am terminating employment and was covered under the NDPERS group health insurance as an active employee, or individuals otherwise losing eligibility, that  I may continue my NDPERS Group Health Coverage at my own expense subject to the following:<br/>

&nbsp;&nbsp;&nbsp;&nbsp;1)	I must be a member of the plan at time of loss of eligibility.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;2)	My spouse or any other dependent(s) applying for this continuation coverage must be a member of the plan at the time of loss of eligibility.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;3)	I must complete and submit this election to NDPERS within 60 days from your last date of coverage.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;4)	I must not have a lapse in coverage, i.e. premiums must be paid to ensure continuous coverage.<br/><br/>

I understand that if I do not choose continuation coverage, my group health insurance coverage will end on the last day of the month for which premiums were paid.<br/><br/>

I understand that if I am eligible, my spouse or my dependents are eligible for Medicare and indicated “Yes” I am eligible, that in <b>order to continue or be eligible for coverage I MUST submit a photocopy of the applicable Medicare ID card(s) for both Parts A & B and complete the NDPERS Medicare Prescription Drug Plan (PDP) Individual Enrollment Form.</b> Therefore, I understand that I or any eligible Medicare member must have Medicare Part B of Medicare.  Application for the Medicare Part D Plan cannot be singed or submitted more than 90 days prior to the requested effective date of coverage.<br/><br/>

I authorize the Social Security Administration to furnish Sanford Health Plan with medical or other information acquired under the Title XVIII Program (MEDICARE) during the periods my contracts are in force. I authorize Sanford Health Plan, or its agent to receive medical information from physicians, hospitals, and other health care providers in order to assure appropriateness of claims payment.<br/><br/>

<b>CANCELLATION POLICY</b><br/>
To cancel NDPERS group insurance coverage, I understand that a written request must be submitted. The request must provide my name, last four digits of social security number, NDPERS Member Id and effective date. A NDPERS Disenrollment form is also required for any individual on Medicare. NDPERS must receive a cancellation request by the end of the month prior to the effective date. Cancellations will only be done at the end of the month. We cannot cancel a policy for a partial month or do a retroactive cancellation of a policy. If canceling coverage, I understand I will be responsible to request reimbursement from RHIC vendor for my retiree health insurance credit, if any.<br/><br/>

I have read and reviewed my eligibility for coverage, the cancellation policy and certify the information is accurate and complete. I understand and agree that any false statements or omissions may constitute a fraudulent act or intentional misrepresentation and may void or retroactively cancel any benefit issued based on this application.' 
WHERE SCREEN_STEP_VALUE='HP'
END

/*Dental plan acknoledgement*/
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='DP' AND ACKNOWLEDGEMENT_TEXT ='I, <b>{0}</b>, acknowledge I have had an opportunity to review the terms and conditions relating to participation in Dental' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = '<b>CANCELLATION POLICY</b><br/>
I understand to cancel NDPERS group insurance coverage, a written request must be submitted. The request must provide my name, last four digits of social security number, NDPERS Member Id and effective date. NDPERS must receive a cancellation request by the end of the month prior to the effective date. Cancellations will only be done at the end of the month. We cannot cancel a policy for a partial month or do a retroactive cancellation of a policy.<br/><br/>

To the best of my knowledge and belief, the information I have provided on this form is correct. I understand that any person who knowingly and with intent to defraud, submits an application or files a claim containing any materially false or misleading information, commits a fraudulent act, which is a crime. I understand my coverage begins on the effective date assigned by the carrier. If canceling coverage, I understand I will be responsible to request reimbursement from RHIC vendor for my retiree health insurance credit, if any.<br/><br/>

I have read this application in its entirety and certify the information is accurate and complete. I understand and agree that any false statements or omissions may void any benefit plans insured based on this application.' 
WHERE SCREEN_STEP_VALUE='DP'
END

/*Vision plan acknoledgement*/
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='VP' AND ACKNOWLEDGEMENT_TEXT ='I, <b>{0}</b>, acknowledge I have had an opportunity to review the terms and conditions relating to participation in Vision' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = '<b>CANCELLATION POLICY</b><br/>
I understand to cancel NDPERS group insurance coverage, a written request must be submitted. The request must provide my name, last four digits of social security number, NDPERS Member Id and effective date. NDPERS must receive a cancellation request by the end of the month prior to the effective date. Cancellations will only be done at the end of the month. We cannot cancel a policy for a partial month or do a retroactive cancellation of a policy.<br/><br/>

To the best of my knowledge and belief, the information I have provided on this form is correct. I understand that any person who knowingly and with intent to defraud, submits an application or files a claim containing any materially false or misleading information, commits a fraudulent act, which is a crime. I understand my coverage begins on the effective date assigned by the carrier. If canceling coverage, I understand I will be responsible to request reimbursement from RHIC vendor for my retiree health insurance credit, if any.<br/><br/>

I have read this application in its entirety and certify the information is accurate and complete. I understand and agree that any false statements or omissions may void any benefit plans insured based on this application.' 
WHERE SCREEN_STEP_VALUE='VP'
END

/*Medicare Part D plan acknoledgement*/
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='MP' AND ACKNOWLEDGEMENT_TEXT ='I, <b>{0}</b>, acknowledge I have had an opportunity to review the terms and conditions relating to participation in Medicare' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = '<b>Express Scripts Medicare</b>® (PDP) is offered by Medco Containment Life Insurance Company, which
contracts with the Federal government. This coverage is Medicare Part D coverage and is in addition to
your coverage under Medicare Parts A and B. You must keep your Medicare Parts A and/or B coverage in
order to qualify for this plan. You must inform your former employer of any other prescription drug coverage
you may have.<br/><br/>

You can be in only one Medicare prescription drug plan at a time. If you are currently in a Medicare
prescription drug plan, a Medicare Advantage Plan with prescription drug coverage, or an individual
Medicare Advantage Plan, your enrollment in Express Scripts Medicare may end that enrollment.
You can join a new Medicare prescription drug plan or Medicare health plan from October 15 to December
7. Except in special cases, you cannot join a new plan at any other time of the year. If you leave this plan
and don’t have or get other Medicare prescription drug coverage or creditable coverage (as good as
Medicare’s), you may be required to pay a late enrollment penalty (LEP) if you go 63 days or more without
Medicare Part D coverage or other creditable prescription drug coverage.<br/><br/>

Some people may have to pay an extra premium amount because of their yearly income. If you have to
pay an extra amount, the Social Security Administration – not your Medicare plan – will send you a letter
telling you what that extra amount will be and how to pay it. If you have any questions about this extra
amount, contact the Social Security Administration at 1.800.772.1213. TTY users call 1.800.325.0778.
Medicare beneficiaries with low or limited income and resources may qualify for Extra Help. If you qualify,
your Medicare prescription drug plan costs will be less. Once you are enrolled in this drug plan, Medicare
will tell the plan how much assistance you will receive and Express Scripts will send you information on the
amount you will pay. If you are not currently receiving Extra Help, you can contact 1.800.MEDICARE
(1.800.633.4227) to see if you might qualify. TTY users call 1.877.486.2048.<br/><br/>

Once you are a member of this plan, you have the right to file a grievance or appeal plan decisions about
payment or services if you disagree. Read your Evidence of Coverage to know which rules you must follow
to receive coverage with this Medicare prescription drug plan.<br/><br/>

This information is not a complete description of benefits. Contact Express Scripts Medicare for more
information. Limitations, copayments and restrictions may apply. Benefits, premium (if applicable) and/or
copayments/coinsurance may change on January 1 of each year. The formulary and/or pharmacy network
may change at any time. You will receive notice when necessary.<br/><br/>

<b>Release of Information</b><br/>
By joining this Medicare prescription drug plan, I acknowledge that Express Scripts Medicare can release
my information to Medicare and other plans as is necessary for treatment, payment and health care
operations.<br/><br/>

I also acknowledge that Express Scripts Medicare can release my information, including my prescription
drug event data, to Medicare, who may release it for research and other purposes that follow all applicable
Federal statutes and regulations.<br/><br/>

I understand this enrollment cannot be signed or submitted more than 90 days prior to the effective date of coverage.' 
WHERE SCREEN_STEP_VALUE='MP'
END

/*Life plan acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='LP')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'LP','Life Plan selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'LP' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'LP',1,'LP', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='LP' AND ACKNOWLEDGEMENT_TEXT ='LP' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'I authorize all physicians and other medical professional, hospitals, and other medical care institution, insurers, medical or hospital service and prepaid health plans, employers and group policyholders, contract holders or benefit plan administrators to provide Voya Financial and any benefit plan administrator, consumer reporting agencies, attorneys and independent claim administrators action Voya Financial behalf with information concerning medical care, advice, treatment or supplies provide the patient including information on mental illness and any employment related information regarding the Patient. This information will be used for the purpose of evaluating
	and administering claims for benefits. I understand the carrier will offer to port my term life policy(ies) or convert to a whole life policy(ies). I understand that if I elect to continue my coverage through NDPERS, I cannot port or convert the coverage with the carrier.<br/><br/>

	I have reviewed the information in its entirety and certify the information is accurate and complete. I understand and agree that any false statements or omissions may void any Benefit Plans insured based on this application.' 
	WHERE SCREEN_STEP_VALUE='LP'
END

/*PLSO plan acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='PLSO')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'PLSO','PLSO selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'PLSO' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'PLSO',1,'PLSO', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='PLSO' AND ACKNOWLEDGEMENT_TEXT ='PLSO' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'The Partial Lump Sum Option (PLSO) is NOT available to early and disabled retirees, or surviving spouses. The PLSO allows you to take a partial lump sum distribution equal to 12 monthly payments determined under the Single Life/Normal benefit option. (No variations will be accepted). If this option is elected, your monthly benefit will be actuarially reduced.<br/><br/>

You will still be permitted to choose one of the optional forms of payment for your ongoing monthly benefit with exception of the Graduated and Deferred Normal Retirement Options. In addition, the PLSO payment, as well as your ongoing monthly benefits, will be subtracted from your individual minimum guarantee.<br/><br/>

This option is a once in a life time election and made at the time of your initial retirement. You may not make an election after receiving your first retirement check nor apply for a second PLSO upon subsequent reemployment and retirement. Please read the <b>“Special Tax Notice Regarding Plan Payments”</b> before continuing. Under Federal law, NDPERS is required to provide this information a minimum of 30 days prior to a distribution. This may affect the date of your PLSO payment.<br/><br/>

I have reviewed and understand the above provisions, and hereby elect the Partial Lump Sum Option. I understand my election is irrevocable and that the Partial Lump Sum option is a once in a life-time election.' 
WHERE SCREEN_STEP_VALUE='PLSO'
END

/*GBO plan acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='GB')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'GB','GB selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'GB' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'GB',1,'GB', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='GB' AND ACKNOWLEDGEMENT_TEXT ='GB' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'The Graduated Benefit Option is NOT available to early and disabled retirees, or surviving spouses. The Graduated Benefit Option allows you to select either a one percent or two percent annual benefit increase. (No variations will be accepted). If this option is elected, your monthly benefit will be actuarially reduced. You will still be permitted to choose one of the optional forms of payment for your ongoing monthly benefit with exception of the Partial Lump Sum option and Deferred Normal Retirement option.<br/><br/>

This option is a once in a life time election and made at the time of your initial retirement. You may not make an election after receiving your initial benefit payment. If you return to work, your Graduated Benefit Option will be applied to your subsequent retirement.<br/><br/>

I have reviewed and understand the Graduated Benefit Option. I understand that the Graduated Benefit Option is a once in a life-time election and my election is irrevocable.' 
WHERE SCREEN_STEP_VALUE='GB'
END

/*Sick Leave Conversion acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='SLC')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'SLC','Sick Leave Conversion selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'SLC' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'SLC',1,'SLC', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='SLC' AND ACKNOWLEDGEMENT_TEXT ='SLC' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'I understand that I have the opportunity to convert any unused sick leave that I accrued with my employer as of my termination date or date that I am no longer eligible to accrue sick leave. Payments can be made to NDPERS as an after-tax payment through a personal check or as a pre-tax payment through a direct rollover or trustee-to-trustee transfer of an eligible fund towards the retirement portion of the sick leave conversion. I have had the opportunity to speak to a financial planner and NDPERS regarding my election and to ask any questions I may have concerning this election. I understand that this election must be made prior to disbursement of any retirement benefits.  I understand that the final cost for the sick leave conversion will be calculated upon my termination. If there is a difference between the sick leave balance or conversion payment amount and the amount that I paid through pre-tax funds, only the amount of sick leave available as of the date of termination will be added to my member record. <b>I understand that the deadline for payment is the 15th of the month following my month of termination or month that I am no longer eligible to accrue sick leave to avoid any delay in my first benefit payment.</b><br/><br/>

<b>I understand this application must be submitted on or before the last working day of the month in which I terminate employment or date that I am no longer eligible to accrue sick leave. Late applications will be VOID.</b>' 
WHERE SCREEN_STEP_VALUE='SLC'
END
/*Flex plan acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='FP')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'FP','FlexComp Plan acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'FP' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'FP',1,'FP', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END

IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='FP' AND ACKNOWLEDGEMENT_TEXT ='FP' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = '<b>Entitlement to COBRA Coverage</b><br/>
Under provisions of the Internal Revenue Service (IRS) COBRA regulations, you have the opportunity to
extend your participation in the Medical Spending Account to the end of the current plan year.
The employer has the responsibility to notify NDPERS of a participant’s death, termination, or reduction in
hours of employment.<br/><br/>

Qualified Beneficiaries Your spouse or dependent(s) may elect to continue coverage in a medical spending
account under the following circumstances:<br/>

&nbsp;&nbsp;&nbsp;&nbsp;1)	Participant’s death.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;2)	Divorce or legal separation.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;3)	A dependent child ceases to be a “dependent child” under the group health plan.<br/><br/>

If you elect COBRA continuation, your premium payment will be based on the annual election amount in
existence at the time of the qualifying event.<br/><br/>

Under the law, it is the responsibility of the person seeking continuation coverage to inform NDPERS of a
divorce, legal separation or a child losing dependent status within 60 days of the date of the event. If you are
interested in COBRA continuation coverage, contact NDPERS for more information.<br/><br/>

<b>Length of COBRA Coverage</b><br/>
You, your spouse or dependent(s), are eligible to receive continuation coverage until the end of the plan
year, or December 31, in which the qualifying event occurred. If you have paid your premium through the
end of the year on December 31 and have a balance in your account, you have the option to have eligible
expenses incurred during the “grace period”, from January 1 through March 15 of the new plan year,
reimbursed from that remaining balance. You will have until April 30 to submit claims. Any amount
remaining in your medical spending reimbursement account after the April 30 claims filing deadline is
forfeited.<br/><br/>

<b>COBRA Coverage Premiums</b>
Employees who elect COBRA continuation coverage are permitted to pre-tax the COBRA premium and prepay
the premium through the end of the current plan year from their final paychecks.
To pay the premium with after-tax dollars throughout the plan year, submit the premium amount plus a two
percent (2%) administrative fee by the first of each month. If you fail to pay the premium on time, your
coverage will terminate on the last day of the month for which a contribution was received.<br/><br/>

<b>Continuation coverage under COBRA is provided subject to your eligibility. NDPERS reserves the
right to terminate your COBRA coverage retroactively if you are determined to be ineligible for
coverage.</b><br/><br/>

You will have 60 days from the date of this notice to inform NDPERS that you want continuation coverage.
<b>IF YOU DO NOT RETURN THIS ELECTION FORM WITHIN 60 DAYS OF THE DATE OF THIS
NOTICE YOU WILL LOSE YOUR RIGHT TO ELECT CONTINUATION COVERAGE</b><br/><br/>

I have read the information in its entirety and agree to abide by the terms of the Plan Document. I understand that if I have elected to pre-pay the premium from my final paychecks, that NDPERS will contact my employer to notify them of my election and to discuss termination processing. <br/><br/>

I certify, under penalties of perjury, that the information submitted on this form is true, correct and complete.' 
WHERE SCREEN_STEP_VALUE='FP'
END

/*App wizard Defined Benefit Plans selected acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='APDB')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'APDB','App wizard Defined Benefit Plans selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'APDB' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'APDB',1,'APDB', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='APDB' AND ACKNOWLEDGEMENT_TEXT ='APDB' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'I elect to receive the retirement benefits and health credit as indicated. I understand I <u>must submit a photocopy of my birth certificate</u>.  (If married, I must also submit a photocopy of spouse’s birth certificate & marriage certificate.)  This Application must be submitted within 6 months of my retirement date and <b><u>must be on file at least 30 days prior to the first retirement payment being issued</u>.  Late applications will result in a delayed retirement effective date.</b>' 
WHERE SCREEN_STEP_VALUE='APDB'
END

/*App wizard Defined Contribution Plans selected acknoledgement*/
IF NOT EXISTS (SELECT 1 FROM [SGS_CODE_VALUE] WHERE CODE_ID = 6000 AND CODE_VALUE='APDC')
INSERT INTO [SGS_CODE_VALUE]([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3]
           ,[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY]
           ,[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES(6000,'APDC','App wizard Defined Contribution Plans selected acknowledgement',NULL,NULL,NULL,NULL,NULL,NULL,NULL
           ,NULL,'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
GO
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'APDC' )
BEGIN
	INSERT INTO SGT_WSS_ACKNOWLEDGEMENT (EFFECTIVE_DATE,SCREEN_STEP_ID,SCREEN_STEP_VALUE,DISPLAY_SEQUENCE,ACKNOWLEDGEMENT_TEXT,SHOW_CHECK_BOX_FLAG,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES('01/01/2018',6000,'APDC',1,'APDC', 'N', 'PIR 18493',GETDATE(),'PIR 18493',GETDATE(),0)
END
IF EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE='APDC' AND ACKNOWLEDGEMENT_TEXT ='APDC' )
BEGIN
	UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT = 'I elect to receive the retirement benefits and health insurance credit as indicated in PART B. I understand I must submit a <u>photocopy of my birth certificate</u>. <b>(If married, also submit a photocopy of spouse’s birth certificate & marriage certificate.)</b><br/><br/>
<b>I understand that this application must be submitted to NDPERS at least 30 days before distribution of my first retirement payment.</b>' 
WHERE SCREEN_STEP_VALUE='APDC'
END

/********************Purpose: PIR 23130  ******************************
*********************Created By: Ganesh Bhor ********************************
*********************Comments: Added new Message *****************/

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10408)
BEGIN
INSERT INTO SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10408,'Verify if RHIC reduction applies for RTW member',16,'W',NULL,NULL,'PIR23130',GETDATE(),'PIR23130',GETDATE(),0)
END
GO 

