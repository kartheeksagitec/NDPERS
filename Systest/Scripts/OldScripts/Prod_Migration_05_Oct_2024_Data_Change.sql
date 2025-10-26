--------------------------------Created By: Saylee P--------------------------
--------------------------------Purpose: PIR 26750----------------------------

IF NOT EXISTS(SELECT* FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10510)
INSERT INTO[DBO].[SGS_MESSAGES] ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],
[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(10510,'Cannot contribute to the HSA as HDHP is not enrolled as of the effective date entered.',16,'E', NULL, NULL,'PIR 26750', GETDATE(),'PIR 26750', GETDATE(),0)
GO

--------------------------------Created By: Saylee P--------------------------
--------------------------------Purpose: PIR 26747----------------------------

IF EXISTS (SELECT 1 FROM [DBO].[SGS_MESSAGES] WHERE [MESSAGE_ID] = 10095 AND DISPLAY_MESSAGE = 'Dependents need to be Enrolled when Family coverage is selected.' )
BEGIN
UPDATE [DBO].[SGS_MESSAGES] SET [DISPLAY_MESSAGE] = 'Dependents need to be Enrolled when Individual and Child(ren) or Family coverage is selected.', 
MODIFIED_BY = 'PIR 26747',MODIFIED_DATE = GETDATE()  
WHERE [MESSAGE_ID] = 10095
END


IF NOT EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_ACKNOWLEDGEMENT] WHERE [ACKNOWLEDGEMENT_ID] = 189 )
BEGIN
SET IDENTITY_INSERT [dbo].[SGT_WSS_ACKNOWLEDGEMENT] ON
INSERT [dbo].[SGT_WSS_ACKNOWLEDGEMENT] ([ACKNOWLEDGEMENT_ID], [EFFECTIVE_DATE], [SCREEN_STEP_ID], [SCREEN_STEP_VALUE], [DISPLAY_SEQUENCE], [ACKNOWLEDGEMENT_TEXT], [SHOW_CHECK_BOX_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (189, CAST(N'2024-09-09T08:46:01.653' AS DateTime), 6000, N'HSA', 2, N'
<span style="font: bold 14px Roboto, sans-serif;">2024 IRS Contribution Limits</span>
<table class="s-grid fluid-table" style="width: 0%;">
	<tr>
		<th style="white-space: nowrap;">Coverage</th>
		<th style="white-space: nowrap;">Annual Limit*</th>
		<th style="white-space: nowrap;">NDPERS Contribution**</th>
		<th style="white-space: nowrap;">Maximum Employee Contribution</th>
	</tr>
	<tr>
		<th>Single</th>
		<td style="text-align: right;">$4,150.00</td>
		<td style="text-align: right;">$1,220.88</td>
		<td style="text-align: right;">$2,929.12</td>
	</tr>  
	<tr>
		<th>Family</th>
		<td style="text-align: right;">$8,300.00</td>
		<td style="text-align: right;">$2,953.92</td>
		<td style="text-align: right;">$5,346.08</td>
	</tr>  
	<tr>
		<th>55+ (Single or Family)</th>
		<td>&nbsp;</td>
		<td>&nbsp;</td>
		<td style="text-align: right;">$1,000 extra</td>
	</tr>
</table>
<span style="font: bold 14px Roboto, sans-serif !important;">2025 IRS Contribution Limits</span>
<table class="s-grid fluid-table" style="width: 0%;">
	<tr>
		<th style="white-space: nowrap;">Coverage</th>
		<th style="white-space: nowrap;">Annual Limit*</th>
		<th style="white-space: nowrap;">NDPERS Contribution**</th>
		<th style="white-space: nowrap;">Maximum Employee Contribution</th>
	</tr>
	<tr>
		<th>Single</th>
		<td style="text-align: right;">$4,300.00</td>
		<td style="text-align: right;">$1,220.88</td>
		<td style="text-align: right;">$3,079.12</td>
	</tr>  
	<tr>
		<th>Family</th>
		<td style="text-align: right;">$8,550.00</td>
		<td style="text-align: right;">$2,953.92</td>
		<td style="text-align: right;">$5,596.08</td>
	</tr>  
	<tr>
		<th>55+ (Single or Family)</th>
		<td>&nbsp;</td>
		<td>&nbsp;</td>
		<td style="text-align: right;">$1,000 extra</td>
	</tr>
</table>
<p style="font: italic 12px Roboto, sans-serif;">*If you are newly electing to participate, your annual limit (PERS and employee contributions) is prorated for the first calendar year based upon the number of months you are participating.<br/>
**The NDPERS Contribution amounts are subject to change July 1 of odd years based upon premiums for the new contract period.</caption></p>
<p>These limits include all contributions (both employee & employer paid) for the calendar year.  I understand that If I exceed the annual limits, it will be my responsibility to request a refund from the HSA administrator or be subject to federal excise tax. </p><p>If my employer allows pre-tax payroll deductions to my Health Savings Account, I elect to defer a monthly amount as stated above.</p><p>I understand that I may modify my election at any time throughout the year as long as applicable payroll timelines are followed.</p><p>I understand that if I am joining the HDHP due to annual enrollment and currently participate in my employer’s Flex Medical Spending Account (MSA), my deduction to my HSA will begin no sooner than February and may be delayed until April if my MSA is not exhausted as of December 31.</p><p>I also understand that if this is the case, the amount I may defer annually to my HSA will be prorated based on the limits and the number of months eligible.</p>
', N'N', N'PIR26436', CAST(N'2024-09-09T08:46:01.653' AS DateTime), N'PIR26436', CAST(N'2024-09-09T08:46:01.653' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[SGT_WSS_ACKNOWLEDGEMENT] OFF
END


--------------Created By: sanket 
--------------Purpose   : PIR 26511
IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 401 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 401
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 407 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 407
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 476 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 476
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 474 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 474
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 472 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 472
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 458 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 458
END
GO

IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 455 AND ROLE_ID != 179)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 179 WHERE ACTIVITY_ID = 455
END
GO